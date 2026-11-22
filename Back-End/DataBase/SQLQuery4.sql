CREATE TABLE tipo_usuario (
    ID INT IDENTITY(1,1) PRIMARY KEY,
    Nome VARCHAR(50) NOT NULL UNIQUE   -- Admin, Funcionario, Cliente
);
GO

INSERT INTO tipo_usuario (Nome) VALUES 
('Admin'),
('Funcionario'),
('Cliente');
GO

ALTER TABLE Usuarios
ADD CONSTRAINT FK_Usuarios_tipo_usuario
    FOREIGN KEY (tipo_usuario) REFERENCES tipo_usuario(ID);
GO