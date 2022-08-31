CREATE TABLE GrupoClasificacion (
    IdGrupoClasificacion INT IDENTITY(1, 1)
                             PRIMARY KEY ,
    Grupo NVARCHAR(100) );
CREATE TABLE Clasificacion (
    IdClasificacion INT IDENTITY(1, 1)
                        PRIMARY KEY ,
    IdGrupo INT NOT NULL ,
    Codigo NVARCHAR(5) NOT NULL ,
    Nombre NVARCHAR(100) NOT NULL ,
    FOREIGN KEY ( IdGrupo ) REFERENCES GrupoClasificacion ( IdGrupoClasificacion ) );

CREATE TABLE Correo (
    IdCorreo INT IDENTITY(1, 1)
                 PRIMARY KEY ,
    Correo NVARCHAR(360) NOT NULL ,
    IdClasificacion INT NOT NULL FOREIGN KEY ( IdClasificacion ) REFERENCES Clasificacion ( IdClasificacion ) );

CREATE TABLE Pais (
    IdPais INT IDENTITY(1, 1)
               PRIMARY KEY ,
    Pais NVARCHAR(100) NOT NULL );

CREATE TABLE Direccion (
    IdDireccion INT IDENTITY(1, 1)
                    PRIMARY KEY ,
    Linea1 NVARCHAR(250) NOT NULL ,
    Linea2 NVARCHAR(250) NOT NULL
                         DEFAULT '' ,
    Referencia NVARCHAR(250) NOT NULL
                             DEFAULT '' ,
    CodigoPostal INT NOT NULL ,
    IdPais INT NOT NULL FOREIGN KEY ( IdPais ) REFERENCES Pais ( IdPais ) );

CREATE	TABLE EnumTexto (
    IdEnumTexto INT IDENTITY(1, 1)
                    PRIMARY KEY ,
    IdGrupo INT NOT NULL ,
    Valor NVARCHAR(100) NOT NULL );


CREATE TABLE ArtefactoCFDI (
    IdArtefactoCFDI INT IDENTITY(1, 1)
                        PRIMARY KEY ,
    Tipo INT NOT NULL ,
    Clave NVARCHAR(10) ,
    Descripcion NVARCHAR(100) NOT NULL ,
    ApTipoPersona INT NOT NULL ,
    FOREIGN KEY ( Tipo ) REFERENCES EnumTexto ( IdEnumTexto ) );

CREATE TABLE DatosFacturacion (
    IdDatoFacturacion INT IDENTITY(1, 1)
                          PRIMARY KEY ,
    TipoPersona INT NOT NULL ,
    RFC NVARCHAR(13) NOT NULL ,
    RazonSocial NVARCHAR(250) NOT NULL ,
    MetodoPago INT NOT NULL ,
    UsoCFDI INT NOT NULL ,
    RegimenFiscal INT NOT NULL ,
    IdDireccion INT NOT NULL ,
    EsSucursal BIT NOT NULL ,
    FOREIGN KEY ( TipoPersona ) REFERENCES Clasificacion ( IdClasificacion ) ,
    FOREIGN KEY ( MetodoPago ) REFERENCES Clasificacion ( IdClasificacion ) ,
    FOREIGN KEY ( UsoCFDI ) REFERENCES ArtefactoCFDI ( IdArtefactoCFDI ) ,
    FOREIGN KEY ( RegimenFiscal ) REFERENCES ArtefactoCFDI ( IdArtefactoCFDI ) ,
    FOREIGN KEY ( IdDireccion ) REFERENCES Direccion ( IdDireccion ) );


CREATE	 TABLE RelDatosFacturacion_Correos (
    IdRelDF_C INT IDENTITY(1, 1)
                  PRIMARY KEY ,
    IdDatoFacturacion INT NOT NULL ,
    IdCorreo INT NOT NULL ,
    FOREIGN KEY ( IdDatoFacturacion ) REFERENCES DatosFacturacion ( IdDatoFacturacion ) ,
    FOREIGN KEY ( IdCorreo ) REFERENCES Correo ( IdCorreo ) );

CREATE TABLE EntidadTributaria (
    IdEntidadTributaria INT IDENTITY(1, 1)
                            PRIMARY KEY ,
    IdClasificacion INT NOT NULL ,
    FOREIGN KEY ( IdClasificacion ) REFERENCES Clasificacion ( IdClasificacion ) );

CREATE TABLE RelEntidadTriburaria_DatosFacturacion (
    IdRelET_DF INT IDENTITY
                   PRIMARY KEY ,
    IdEntidadTributaria INT NOT NULL ,
    IdDatoFacturacion INT NOT NULL ,
    FOREIGN KEY ( IdEntidadTributaria ) REFERENCES EntidadTributaria ( IdEntidadTributaria ) ,
    FOREIGN KEY ( IdDatoFacturacion ) REFERENCES DatosFacturacion ( IdDatoFacturacion ) );
