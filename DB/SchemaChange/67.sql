IF NOT EXISTS (SELECT TOP 1 1 FROM sys.columns WHERE name='Seq' AND object_id=object_id('Services'))
BEGIN
	ALTER TABLE Services
	ADD Seq INT
END