SET QUOTED_IDENTIFIER ON
SET ANSI_NULLS ON
GO
CREATE PROCEDURE spAgregarEditarDatosFacturacion
    @IdDatoFacturacion INT = 0 ,
    @TipoPersona INT ,
    @RFC NVARCHAR(13) ,
    @RazonSocial NVARCHAR(250) ,
    @MetodoPago INT = 22 ,
    @UsoCFDI INT ,
    @RegimenFiscal INT ,
    @IdDireccion INT = 1 ,
    @EsSucursal BIT ,
    @TipoEntidadTributaria INT ,
    @Correo NVARCHAR(360)
AS
    BEGIN
        DECLARE @IdEntidadTributaria INT = 0;
        DECLARE @IdCorreo INT;
        DECLARE @IdRelDF_C INT;

        IF ( @IdDatoFacturacion = 0 )
            BEGIN
				--Insertamos el dato de facturacion	
                INSERT  INTO DatosFacturacion ( TipoPersona, RFC, RazonSocial, MetodoPago, UsoCFDI, RegimenFiscal,
                                                IdDireccion, EsSucursal )
                VALUES  ( @TipoPersona, @RFC, @RazonSocial, @MetodoPago, @UsoCFDI, @RegimenFiscal, @IdDireccion,
                          @EsSucursal );
                SELECT  @IdDatoFacturacion = SCOPE_IDENTITY();									

				--Creamos la entidad triburtaria
                INSERT  INTO EntidadTributaria ( IdClasificacion )
                VALUES  ( @TipoEntidadTributaria );
                SELECT  @IdEntidadTributaria = SCOPE_IDENTITY();
				--Relacionamos la entidad con el dato de facturacion
                INSERT  INTO RelEntidadTriburaria_DatosFacturacion ( IdEntidadTributaria, IdDatoFacturacion )
                VALUES  ( @IdEntidadTributaria, @IdDatoFacturacion );
            END;
        ELSE
            BEGIN
                UPDATE  DatosFacturacion
                SET     TipoPersona = @TipoPersona, RFC = @RFC, RazonSocial = @RazonSocial, MetodoPago = @MetodoPago,
                        UsoCFDI = @UsoCFDI, RegimenFiscal = @RegimenFiscal, IdDireccion = @IdDireccion,
                        EsSucursal = @EsSucursal
                WHERE   IdDatoFacturacion = @IdDatoFacturacion;

                SELECT  @IdEntidadTributaria = ET.IdEntidadTributaria
                FROM    dbo.RelEntidadTriburaria_DatosFacturacion RETDF
                        INNER JOIN dbo.EntidadTributaria ET ON ET.IdEntidadTributaria = RETDF.IdEntidadTributaria
                        INNER JOIN dbo.DatosFacturacion DF ON DF.IdDatoFacturacion = RETDF.IdDatoFacturacion
                WHERE   DF.IdDatoFacturacion = @IdDatoFacturacion
                        AND ET.IdClasificacion = @TipoEntidadTributaria;

                SET @IdEntidadTributaria = ISNULL(@IdEntidadTributaria, -1);
                IF @IdEntidadTributaria = -1
                    BEGIN
						--Creamos la entidad triburtaria
                        INSERT  INTO EntidadTributaria ( IdClasificacion )
                        VALUES  ( @TipoEntidadTributaria );
                        SELECT  @IdEntidadTributaria = SCOPE_IDENTITY();
						--Relacionamos la entidad con el dato de facturacion
                        INSERT  INTO RelEntidadTriburaria_DatosFacturacion ( IdEntidadTributaria, IdDatoFacturacion )
                        VALUES  ( @IdEntidadTributaria, @IdDatoFacturacion );
                    END; 

            END;	
			
		--Verificamos si existe el correo que se esta dando de alta
        SELECT  @IdCorreo = C.IdCorreo
        FROM    Correo C
        WHERE   C.Correo = @Correo;

        SET @IdCorreo = ISNULL(@IdCorreo, -1);
		PRINT CONCAT('@IdCorreo: ',@IdCorreo)
        IF @IdCorreo = -1
            BEGIN
				--Si no existe lo creamos y lo relacionamos
                INSERT  INTO dbo.Correo ( Correo, IdClasificacion )
                VALUES  ( @Correo, 121 );
                SELECT  @IdCorreo = SCOPE_IDENTITY();      
                INSERT  INTO dbo.RelDatosFacturacion_Correos ( IdDatoFacturacion, IdCorreo )
                VALUES  ( @IdDatoFacturacion, @IdCorreo );                   
            END;
        ELSE
            BEGIN
                SELECT  @IdRelDF_C = RDFC.IdRelDF_C
                FROM    dbo.RelDatosFacturacion_Correos RDFC
                WHERE   RDFC.IdDatoFacturacion = @IdDatoFacturacion
                        AND RDFC.IdCorreo = @IdCorreo;

                SET @IdRelDF_C = ISNULL(@IdRelDF_C, -1);

                IF @IdRelDF_C = -1
                    BEGIN
                        INSERT  INTO dbo.RelDatosFacturacion_Correos ( IdDatoFacturacion, IdCorreo )
                        VALUES  ( @IdDatoFacturacion, @IdCorreo ); 
                    END; 

            END;
        
    END;
    SELECT  @IdDatoFacturacion AS IdDatoFacturacion;

GO