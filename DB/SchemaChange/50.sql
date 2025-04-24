IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE name = 'QRCodePicture' AND object_id=object_id('Salons'))
BEGIN
	ALTER TABLE Salons
	ADD QRCodePicture char(40)
END