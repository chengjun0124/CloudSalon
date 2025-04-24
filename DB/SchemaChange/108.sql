UPDATE ConsumedServiceStatus SET ConsumedServiceStatusStatus='NeedConfirm' WHERE ConsumedServiceStatusId=1
UPDATE ConsumedServiceStatus SET ConsumedServiceStatusStatus='Confirmed' WHERE ConsumedServiceStatusId=2

IF NOT EXISTS (SELECT 1 FROM ConsumedServiceStatus WHERE ConsumedServiceStatusId=3)
BEGIN
	INSERT INTO ConsumedServiceStatus
	VALUES (3,'Completed')
END

IF NOT EXISTS (SELECT 1 FROM ConsumedServiceStatus WHERE ConsumedServiceStatusId=4)
BEGIN
	INSERT INTO ConsumedServiceStatus
	VALUES (4,'Rejected')
END


UPDATE ConsumedServices SET ConsumedServiceStatusId=3 WHERE ConsumedServiceStatusId IS NULL

ALTER TABLE ConsumedServices
ALTER COLUMN ConsumedServiceStatusId INT NOT NULL