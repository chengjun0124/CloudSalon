IF NOT EXISTS (SELECT TOP 1 1 FROM ServiceTypes WHERE ServiceTypeId=8)
BEGIN
	INSERT INTO ServiceTypes
	VALUES
	(8,'纹绣美甲')
END


IF NOT EXISTS (SELECT TOP 1 1 FROM ServiceTypes WHERE ServiceTypeId=9)
BEGIN
	INSERT INTO ServiceTypes
	VALUES
	(9,'抗敏脱敏')
END

UPDATE ServiceTypes SET ServiceTypeName='紧致抗衰' WHERE ServiceTypeId=4