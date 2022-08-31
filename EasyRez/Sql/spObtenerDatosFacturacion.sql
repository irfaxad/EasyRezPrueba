SET QUOTED_IDENTIFIER ON
SET ANSI_NULLS ON
GO
CREATE PROCEDURE spObtenerDatosFacturacion
    @IdDatoFacturacion AS INT = -1 ,
    @TipoEntidadTributaria INT = -1
AS
    SELECT  DF.IdDatoFacturacion, DF.RFC, DF.RazonSocial, C.Nombre AS MetodoPago, AC.Clave AS UsoCFDIClave,
            AC.Descripcion AS UsoCFDIDescripcion, C2.Nombre AS TipoPersona, AC2.Clave AS RegimenFiscal_Clave,
            AC2.Descripcion AS RegimenFiscal_Descripción, DF.EsSucursal, C3.Correo, C4.Codigo AS EntidadTributariaGrupo,
            C4.Nombre AS EntidadTributariaCodigo, DF.IdDireccion, C4.IdClasificacion AS TipoEntidadTributaria,
            DF.MetodoPago AS IdMetodoPago, DF.TipoPersona AS IdTipoPersona, DF.UsoCFDI, DF.RegimenFiscal, ET.IdEntidadTributaria
    FROM    dbo.EntidadTributaria ET
            INNER JOIN dbo.RelEntidadTriburaria_DatosFacturacion RETDF ON RETDF.IdEntidadTributaria = ET.IdEntidadTributaria
            INNER JOIN dbo.DatosFacturacion DF ON DF.IdDatoFacturacion = RETDF.IdDatoFacturacion
            INNER JOIN dbo.Clasificacion C ON C.IdClasificacion = DF.MetodoPago
            INNER JOIN dbo.ArtefactoCFDI AC ON AC.IdArtefactoCFDI = DF.UsoCFDI
            INNER JOIN dbo.Clasificacion C2 ON C2.IdClasificacion = DF.TipoPersona
            INNER JOIN dbo.ArtefactoCFDI AC2 ON AC2.IdArtefactoCFDI = DF.RegimenFiscal
            INNER JOIN dbo.RelDatosFacturacion_Correos RDFC ON RDFC.IdDatoFacturacion = DF.IdDatoFacturacion
            INNER JOIN dbo.Correo C3 ON C3.IdCorreo = RDFC.IdCorreo
            INNER JOIN dbo.Clasificacion C4 ON C4.IdClasificacion = ET.IdClasificacion
    WHERE   ( DF.IdDatoFacturacion = @IdDatoFacturacion
              OR @IdDatoFacturacion = -1
            )
            AND ( C4.IdClasificacion = @TipoEntidadTributaria
                  OR @TipoEntidadTributaria = -1
                )
    ORDER BY DF.IdDatoFacturacion ASC;


GO