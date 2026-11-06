IF DB_ID(N'GymFlowDb') IS NULL
BEGIN
	CREATE DATABASE GymFlowDb;
END;
GO

USE GymFlowDb;
GO

IF OBJECT_ID(N'dbo.Membresias', N'U') IS NULL
BEGIN
	CREATE TABLE dbo.Membresias
	(
		MembresiaId INT IDENTITY(1,1) NOT NULL,
		Nombre NVARCHAR(60) NOT NULL,
		Descripcion NVARCHAR(250) NULL,
		DuracionMeses SMALLINT NOT NULL,
		Precio DECIMAL(10,2) NOT NULL,
		Activa BIT NOT NULL CONSTRAINT DF_Membresias_Activa DEFAULT 1,
		FechaCreacion DATETIME2 NOT NULL CONSTRAINT DF_Membresias_FechaCreacion DEFAULT SYSUTCDATETIME(),
		CONSTRAINT PK_Membresias PRIMARY KEY (MembresiaId),
		CONSTRAINT UQ_Membresias_Nombre UNIQUE (Nombre),
		CONSTRAINT CK_Membresias_Duracion CHECK (DuracionMeses BETWEEN 1 AND 24),
		CONSTRAINT CK_Membresias_Precio CHECK (Precio > 0)
	);
END;
GO

IF OBJECT_ID(N'dbo.Socios', N'U') IS NULL
BEGIN
	CREATE TABLE dbo.Socios
	(
		SocioId INT IDENTITY(1,1) NOT NULL,
		Dni VARCHAR(8) NOT NULL,
		Nombres NVARCHAR(80) NOT NULL,
		Apellidos NVARCHAR(100) NOT NULL,
		Correo VARCHAR(150) NOT NULL,
		FechaNacimiento DATE NOT NULL,
		FechaRegistro DATETIME2 NOT NULL CONSTRAINT DF_Socios_FechaRegistro DEFAULT SYSUTCDATETIME(),
		MembresiaId INT NOT NULL,
		CONSTRAINT PK_Socios PRIMARY KEY (SocioId),
		CONSTRAINT UQ_Socios_Dni UNIQUE (Dni),
		CONSTRAINT UQ_Socios_Correo UNIQUE (Correo),
		CONSTRAINT FK_Socios_Membresias FOREIGN KEY (MembresiaId)
			REFERENCES dbo.Membresias (MembresiaId)
			ON DELETE NO ACTION
	);
END;
GO

INSERT INTO dbo.Membresias (Nombre, Descripcion, DuracionMeses, Precio, Activa)
SELECT datos.Nombre, datos.Descripcion, datos.DuracionMeses, datos.Precio, datos.Activa
FROM
(
	VALUES
		(N'Plan Basico', N'Acceso a la sala de musculacion en horario regular.', CAST(1 AS SMALLINT), CAST(79.90 AS DECIMAL(10,2)), CAST(1 AS BIT)),
		(N'Plan Premium', N'Acceso completo, clases grupales y asesoria personalizada.', CAST(6 AS SMALLINT), CAST(399.90 AS DECIMAL(10,2)), CAST(1 AS BIT)),
		(N'Plan Anual', N'Acceso completo durante doce meses con precio preferencial.', CAST(12 AS SMALLINT), CAST(699.90 AS DECIMAL(10,2)), CAST(1 AS BIT))
) AS datos(Nombre, Descripcion, DuracionMeses, Precio, Activa)
WHERE NOT EXISTS
(
	SELECT 1
	FROM dbo.Membresias AS membresia
	WHERE membresia.Nombre = datos.Nombre
);

INSERT INTO dbo.Socios (Dni, Nombres, Apellidos, Correo, FechaNacimiento, MembresiaId)
SELECT datos.Dni,
	   datos.Nombres,
	   datos.Apellidos,
	   datos.Correo,
	   datos.FechaNacimiento,
	   membresia.MembresiaId
FROM
(
	VALUES
		('74125836', N'Ana', N'Torres Mendoza', 'ana.torres@example.com', CAST('1998-04-15' AS DATE), N'Plan Basico'),
		('72839451', N'Luis', N'Quispe Rojas', 'luis.quispe@example.com', CAST('1995-09-22' AS DATE), N'Plan Premium'),
		('70518264', N'Carla', N'Ramirez Flores', 'carla.ramirez@example.com', CAST('2000-01-30' AS DATE), N'Plan Anual'),
		('76984512', N'Diego', N'Vargas Salazar', 'diego.vargas@example.com', CAST('1997-11-08' AS DATE), N'Plan Basico'),
		('71362948', N'Maria', N'Huaman Castro', 'maria.huaman@example.com', CAST('1999-06-18' AS DATE), N'Plan Premium')
) AS datos(Dni, Nombres, Apellidos, Correo, FechaNacimiento, NombreMembresia)
INNER JOIN dbo.Membresias AS membresia
	ON membresia.Nombre = datos.NombreMembresia
WHERE NOT EXISTS
(
	SELECT 1
	FROM dbo.Socios AS socio
	WHERE socio.Dni = datos.Dni
	   OR socio.Correo = datos.Correo
);
GO

SELECT * FROM dbo.Membresias;
SELECT * FROM dbo.Socios;
