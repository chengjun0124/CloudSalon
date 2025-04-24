IF NOT EXISTS(SELECT TOP 1 1 FROM sys.columns WHERE name='IsBeautician' AND object_id=object_id('Employees'))
BEGIN
	ALTER TABLE Employees
	ADD IsBeautician BIT
	
	exec sp_executesql N'UPDATE Employees set IsBeautician=0 WHERE UserTypeId=4 or UserTypeId=5'
END