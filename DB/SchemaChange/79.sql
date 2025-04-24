--IF EXISTS (SELECT TOP 1 1 FROM sys.columns WHERE name = 'UpdatedDate' AND object_id=object_id('Services'))
--BEGIN
--	ALTER TABLE Services
--	DROP COLUMN UpdatedDate
--END

--IF NOT EXISTS (SELECT TOP 1 1 FROM sys.columns WHERE name = 'RootServiceId' AND object_id=object_id('Services'))
--BEGIN
--	ALTER TABLE Services
--	ADD RootServiceId INT
--END

--IF NOT EXISTS (SELECT TOP 1 1 FROM sys.foreign_keys  where name='FK_Services_Services_ServiceId')
--BEGIN
--	ALTER TABLE Services  WITH CHECK ADD CONSTRAINT FK_Services_Services_ServiceId FOREIGN KEY(RootServiceId)
--	REFERENCES Services (ServiceId)
	
--	ALTER TABLE Services CHECK CONSTRAINT FK_Services_Services_ServiceId
--END

--IF NOT EXISTS (SELECT TOP 1 1 FROM sys.columns WHERE name = 'IsLastEdit' AND object_id=object_id('Services'))
--BEGIN
--	ALTER TABLE Services
--	ADD IsLastEdit BIT
	
--	EXEC sp_executesql N'UPDATE Services SET IsLastEdit=1'
	
--	ALTER TABLE Services
--	ALTER COLUMN IsLastEdit BIT NOT NULL
--END

--以上代码用于不新建快照表ServiceSnapShots，使用Services表本身实现快照功能

--改正错误的外键名
IF EXISTS (SELECT TOP 1 1 FROM sys.foreign_keys  where name='FK_dbo.Employees_dbo.Salons_SalonID')
BEGIN
	ALTER TABLE Employees
	DROP constraint [FK_dbo.Employees_dbo.Salons_SalonID]
END

IF EXISTS (SELECT TOP 1 1 FROM sys.foreign_keys  where name='FK_dbo.Employees_dbo.UserTypes_UserTypeID')
BEGIN
	ALTER TABLE Employees
	DROP constraint [FK_dbo.Employees_dbo.UserTypes_UserTypeID]
END

IF EXISTS (SELECT TOP 1 1 FROM sys.foreign_keys  where name='FK_dbo.ServiceImages_dbo.Services_ServiceID')
BEGIN
	ALTER TABLE ServiceEffectImages
	DROP constraint [FK_dbo.ServiceImages_dbo.Services_ServiceID]
END

IF NOT EXISTS (SELECT TOP 1 1 FROM sys.foreign_keys  where name='FK_Employees_Salons_SalonId')
BEGIN
	ALTER TABLE Employees  WITH CHECK ADD CONSTRAINT FK_Employees_Salons_SalonId FOREIGN KEY(SalonId)
	REFERENCES Salons (SalonId)
	
	ALTER TABLE Employees CHECK CONSTRAINT FK_Employees_Salons_SalonId
END

IF NOT EXISTS (SELECT TOP 1 1 FROM sys.foreign_keys  where name='FK_Employees_UserTypes_UserTypeId')
BEGIN
	ALTER TABLE Employees  WITH CHECK ADD CONSTRAINT FK_Employees_UserTypes_UserTypeId FOREIGN KEY(UserTypeId)
	REFERENCES UserTypes (UserTypeId)
	
	ALTER TABLE Employees CHECK CONSTRAINT FK_Employees_UserTypes_UserTypeId
END

IF NOT EXISTS (SELECT TOP 1 1 FROM sys.foreign_keys  where name='FK_ServiceEffectImages_Services_ServiceId')
BEGIN
	ALTER TABLE ServiceEffectImages  WITH CHECK ADD CONSTRAINT FK_ServiceEffectImages_Services_ServiceId FOREIGN KEY(ServiceId)
	REFERENCES Services (ServiceId)
	
	ALTER TABLE ServiceEffectImages CHECK CONSTRAINT FK_ServiceEffectImages_Services_ServiceId
END
--改正错误的外键名

--添加一个遗漏的外键
IF NOT EXISTS (SELECT TOP 1 1 FROM sys.foreign_keys  where name='FK_Services_Salons_SalonId')
BEGIN
	ALTER TABLE Services  WITH CHECK ADD CONSTRAINT FK_Services_Salons_SalonId FOREIGN KEY(SalonId)
	REFERENCES Salons (SalonId)
	
	ALTER TABLE Services CHECK CONSTRAINT FK_Services_Salons_SalonId
END
--添加一个遗漏的外键

IF NOT EXISTS (SELECT TOP 1 1 FROM sys.columns WHERE name = 'EditBy' AND object_id=object_id('Services'))
BEGIN
	ALTER TABLE Services
	ADD EditBy INT
	
	EXEC sp_executesql N'UPDATE s
	SET EditBy = e.EmployeeId
	FROM Services s
	LEFT JOIN (
	--找到各美容院的老板，找不到就找第一个美管
		SELECT
		RANK() OVER 
		(PARTITION BY e.SalonId ORDER BY UserTypeId DESC,EmployeeId) AS RANK,
		SalonId,
		EmployeeId
	FROM Employees e
	WHERE e.IsDeleted=0
	AND UserTypeId=4 or UserTypeId=5) e
	ON s.SalonId = e.SalonId AND e.RANK=1'
	
	ALTER TABLE Services
	ALTER COLUMN EditBy INT NOT NULL
END

IF NOT EXISTS (SELECT TOP 1 1 FROM sys.foreign_keys  where name='FK_Services_Employees_EditBy')
BEGIN
	ALTER TABLE Services  WITH CHECK ADD CONSTRAINT FK_Services_Employees_EditBy FOREIGN KEY(EditBy)
	REFERENCES Employees (EmployeeId)
	
	ALTER TABLE Services CHECK CONSTRAINT FK_Services_Employees_EditBy
END

IF NOT EXISTS (SELECT TOP 1 1 FROM sys.tables WHERE name = 'ServiceSnapShots')
BEGIN
	CREATE TABLE ServiceSnapShots
	(
		ServiceSnapShotId INT IDENTITY(1,1) NOT NULL,
		ServiceId INT NOT NULL,
		ServiceName NVARCHAR(16) NOT NULL,
		ServiceTypeId INT NOT NULL,
		Pain TINYINT NOT NULL,
		Duration INT NOT NULL,
		OncePrice DECIMAL(9,2),
		TreatmentPrice DECIMAL(9,2),
		TreatmentTime TINYINT,
		TreatmentInterval TINYINT,
		Functionality NVARCHAR(200) NOT NULL,
		SuitablePeople NVARCHAR(200),
		Detail NVARCHAR(200),
		CreatedDate DATETIME NOT NULL,
		SubjectImage CHAR(40),
		EditBy INT NOT NULL,
		CONSTRAINT PK_ServiceSnapShotId PRIMARY KEY CLUSTERED 
		(
			ServiceSnapShotId ASC
		)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
	) ON [PRIMARY]
END

IF NOT EXISTS (SELECT TOP 1 1 FROM sys.foreign_keys  where name='FK_ServiceSnapShots_Services_ServiceId')
BEGIN
	ALTER TABLE ServiceSnapShots WITH CHECK ADD CONSTRAINT FK_ServiceSnapShots_Services_ServiceId FOREIGN KEY(ServiceId)
	REFERENCES Services (ServiceId)
	
	ALTER TABLE ServiceSnapShots CHECK CONSTRAINT FK_ServiceSnapShots_Services_ServiceId
END

IF NOT EXISTS (SELECT TOP 1 1 FROM sys.foreign_keys  where name='FK_ServiceSnapShots_ServiceTypes_ServiceTypeId')
BEGIN
	ALTER TABLE ServiceSnapShots WITH CHECK ADD CONSTRAINT FK_ServiceSnapShots_ServiceTypes_ServiceTypeId FOREIGN KEY(ServiceTypeId)
	REFERENCES ServiceTypes (ServiceTypeId)
	
	ALTER TABLE ServiceSnapShots CHECK CONSTRAINT FK_ServiceSnapShots_ServiceTypes_ServiceTypeId
END

IF NOT EXISTS (SELECT TOP 1 1 FROM sys.foreign_keys  where name='FK_ServiceSnapShots_Employees_EditBy')
BEGIN
	ALTER TABLE ServiceSnapShots WITH CHECK ADD CONSTRAINT FK_ServiceSnapShots_Employees_EditBy FOREIGN KEY(EditBy)
	REFERENCES Employees (EmployeeId)
	
	ALTER TABLE ServiceSnapShots CHECK CONSTRAINT FK_ServiceSnapShots_Employees_EditBy
END

EXEC sp_executesql N'INSERT INTO ServiceSnapShots
(
	ServiceId,
	ServiceName,
	ServiceTypeId,
	Pain,
	Duration,
	OncePrice,
	TreatmentPrice,
	TreatmentTime,
	TreatmentInterval,
	Functionality,
	SuitablePeople,
	Detail,
	CreatedDate,
	SubjectImage,
	EditBy
)
SELECT
	s.ServiceId,
	s.ServiceName,
	s.ServiceTypeId,
	s.Pain,
	s.Duration,
	s.OncePrice,
	s.TreatmentPrice,
	s.TreatmentTime,
	s.TreatmentInterval,
	s.Functionality,
	s.SuitablePeople,
	s.Detail,
	s.CreatedDate,
	s.SubjectImage,
	s.EditBy
FROM Services s
LEFT JOIN ServiceSnapShots sss
ON s.ServiceId = sss.ServiceId
WHERE sss.ServiceId IS NULL'

EXEC sp_executesql N'UPDATE Services SET UpdatedDate=null'

EXEC sp_executesql N'UPDATE Services SET UpdatedDate=GETDATE() WHERE IsDeleted=1'

IF EXISTS (SELECT TOP 1 1 FROM sys.columns WHERE name = 'CreatedDate' AND object_id=object_id('ServiceEffectImages'))
BEGIN
	ALTER TABLE ServiceEffectImages
	DROP COLUMN CreatedDate
END

IF NOT EXISTS (SELECT TOP 1 1 FROM sys.tables WHERE name = 'ServiceEffectImageSnapShots')
BEGIN
	CREATE TABLE ServiceEffectImageSnapShots
	(
		ServiceSnapShotId INT NOT NULL,
		[FileName] CHAR(40) NOT NULL,
		Seq INT NOT NULL,
		CONSTRAINT PK_ServiceEffectImageSnapShots PRIMARY KEY CLUSTERED 
		(
			ServiceSnapShotId ASC,
			Seq ASC
		)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
	) ON [PRIMARY]
END

IF NOT EXISTS (SELECT TOP 1 1 FROM sys.foreign_keys  where name='FK_ServiceEffectImageSnapShots_ServiceSnapShots')
BEGIN
	ALTER TABLE ServiceEffectImageSnapShots WITH CHECK ADD CONSTRAINT FK_ServiceEffectImageSnapShots_ServiceSnapShots FOREIGN KEY(ServiceSnapShotId)
	REFERENCES ServiceSnapShots (ServiceSnapShotId)
	
	ALTER TABLE ServiceEffectImageSnapShots CHECK CONSTRAINT FK_ServiceEffectImageSnapShots_ServiceSnapShots
END

INSERT INTO ServiceEffectImageSnapShots
(
	ServiceSnapShotId,
	[FileName],
	Seq
)
select
	sss.ServiceSnapShotId,
	sei.[FileName],
	sei.Seq
from ServiceSnapShots sss
INNER JOIN ServiceEffectImages sei
ON sss.ServiceId = sei.ServiceId
LEFT JOIN ServiceEffectImageSnapShots seiss
ON seiss.ServiceSnapShotId = sss.ServiceSnapShotId
WHERE seiss.ServiceSnapShotId IS NULL


IF NOT EXISTS (SELECT TOP 1 1 FROM sys.columns WHERE name = 'ServiceSnapShotId' AND object_id=object_id('Appointments'))
BEGIN
	ALTER TABLE Appointments
	ADD ServiceSnapShotId INT
	
	EXEC sp_executesql N'UPDATE a
	SET ServiceSnapShotId = sss.ServiceSnapShotId
	FROM Appointments a
	INNER JOIN ServiceSnapShots sss
	ON a.ServiceId = sss.ServiceId'
	
	ALTER TABLE Appointments
	ALTER COLUMN ServiceSnapShotId INT NOT NULL
END

IF NOT EXISTS (SELECT TOP 1 1 FROM sys.foreign_keys  where name='FK_Appointments_ServiceSnapShots_ServiceSnapShotId')
BEGIN
	ALTER TABLE Appointments  WITH CHECK ADD CONSTRAINT FK_Appointments_ServiceSnapShots_ServiceSnapShotId FOREIGN KEY(ServiceSnapShotId)
	REFERENCES ServiceSnapShots (ServiceSnapShotId)
	
	ALTER TABLE Appointments CHECK CONSTRAINT FK_Appointments_ServiceSnapShots_ServiceSnapShotId
END

IF EXISTS (SELECT TOP 1 1 FROM sys.foreign_keys  where name='FK_Appointments_Services_ServiceId')
BEGIN
	ALTER TABLE Appointments
	DROP constraint FK_Appointments_Services_ServiceId
END

IF EXISTS (SELECT TOP 1 1 FROM sys.columns WHERE name = 'ServiceId' AND object_id=object_id('Appointments'))
BEGIN
	ALTER TABLE Appointments
	DROP COLUMN ServiceId
END



--创建表PurchasedServices，及关系
IF NOT EXISTS (SELECT TOP 1 1 FROM sys.tables WHERE name = 'PurchasedServices')
BEGIN
	CREATE TABLE PurchasedServices
	(
		PurchasedServiceId INT IDENTITY(1,1) NOT NULL,
		UserId INT,
		ServiceSnapShotId INT NOT NULL,
		Payment decimal(9,2) NOT NULL,
		Mode tinyint NOT NULL,
		CreatedDate DATETIME NOT NULL,
		[Time] TINYINT NULL,
		OperatorId INT NOT NULL
		CONSTRAINT [PK_PurchasedServiceId] PRIMARY KEY CLUSTERED 
		(
			PurchasedServiceId ASC
		)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
	) ON [PRIMARY]
END

IF NOT EXISTS (SELECT TOP 1 1 FROM sys.foreign_keys  where name='FK_PurchasedServices_Users_UserId')
BEGIN
	ALTER TABLE PurchasedServices  WITH CHECK ADD CONSTRAINT FK_PurchasedServices_Users_UserId FOREIGN KEY(UserId)
	REFERENCES Users (UserId)
	
	ALTER TABLE PurchasedServices CHECK CONSTRAINT FK_PurchasedServices_Users_UserId
END

IF NOT EXISTS (SELECT TOP 1 1 FROM sys.foreign_keys  where name='FK_PurchasedServices_ServiceSnapShots_ServiceSnapShotId')
BEGIN
	ALTER TABLE PurchasedServices  WITH CHECK ADD CONSTRAINT FK_PurchasedServices_ServiceSnapShots_ServiceSnapShotId FOREIGN KEY(ServiceSnapShotId)
	REFERENCES ServiceSnapShots (ServiceSnapShotId)
	
	ALTER TABLE PurchasedServices CHECK CONSTRAINT FK_PurchasedServices_ServiceSnapShots_ServiceSnapShotId
END

IF NOT EXISTS (SELECT TOP 1 1 FROM sys.foreign_keys  where name='FK_PurchasedServices_Employees_OperatorId')
BEGIN
	ALTER TABLE PurchasedServices  WITH CHECK ADD CONSTRAINT FK_PurchasedServices_Employees_OperatorId FOREIGN KEY(OperatorId)
	REFERENCES Employees (EmployeeId)
	
	ALTER TABLE PurchasedServices CHECK CONSTRAINT FK_PurchasedServices_Employees_OperatorId
END
--创建表PurchasedServices，及关系


--创建表ConsumedServices，及关系
IF NOT EXISTS (SELECT TOP 1 1 FROM sys.tables WHERE name = 'ConsumedServices')
BEGIN
	CREATE TABLE ConsumedServices
	(
		ConsumedServiceId INT IDENTITY(1,1) NOT NULL,
		UserId INT NULL,
		EmployeeId INT NOT NULL,
		CreatedDate DATETIME NOT NULL,
		AppointmentId INT,
		OperatorId INT NULL,
		Mode TINYINT NOT NULL,
		ConsumedServiceStatusId INT NULL,
		UserConfirmedDate DATETIME NULL,
		ChangeTimeReason NVARCHAR(200) NULL
		CONSTRAINT [PK_ConsumedServiceId] PRIMARY KEY CLUSTERED 
		(
			ConsumedServiceId ASC
		)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
	) ON [PRIMARY]
END

IF NOT EXISTS (SELECT TOP 1 1 FROM sys.foreign_keys  where name='FK_ConsumedServices_Users_UserId')
BEGIN
	ALTER TABLE ConsumedServices  WITH CHECK ADD CONSTRAINT FK_ConsumedServices_Users_UserId FOREIGN KEY(UserId)
	REFERENCES Users (UserId)
	
	ALTER TABLE ConsumedServices CHECK CONSTRAINT FK_ConsumedServices_Users_UserId
END

IF NOT EXISTS (SELECT TOP 1 1 FROM sys.foreign_keys  where name='FK_ConsumedServices_Employees_EmployeeId')
BEGIN
	ALTER TABLE ConsumedServices  WITH CHECK ADD CONSTRAINT FK_ConsumedServices_Employees_EmployeeId FOREIGN KEY(EmployeeId)
	REFERENCES Employees (EmployeeId)
	
	ALTER TABLE ConsumedServices CHECK CONSTRAINT FK_ConsumedServices_Employees_EmployeeId
END

IF NOT EXISTS (SELECT TOP 1 1 FROM sys.foreign_keys  where name='FK_ConsumedServices_Appointments_AppointmentId')
BEGIN
	ALTER TABLE ConsumedServices  WITH CHECK ADD CONSTRAINT FK_ConsumedServices_Appointments_AppointmentId FOREIGN KEY(AppointmentId)
	REFERENCES Appointments (AppointmentId)
	
	ALTER TABLE ConsumedServices CHECK CONSTRAINT FK_ConsumedServices_Appointments_AppointmentId
END

IF NOT EXISTS (SELECT TOP 1 1 FROM sys.foreign_keys  where name='FK_ConsumedServices_Employees_OperatorId')
BEGIN
	ALTER TABLE ConsumedServices  WITH CHECK ADD CONSTRAINT FK_ConsumedServices_Employees_OperatorId FOREIGN KEY(OperatorId)
	REFERENCES Employees (EmployeeId)
	
	ALTER TABLE ConsumedServices CHECK CONSTRAINT FK_ConsumedServices_Employees_OperatorId
END
--创建表ConsumedServices，及关系

IF NOT EXISTS (SELECT TOP 1 1 FROM sys.columns WHERE name = 'IsActive' AND object_id=object_id('Users'))
BEGIN
	ALTER TABLE Users
	ADD IsActive BIT
	
	EXEC sp_executesql N'UPDATE Users SET IsActive=1'
	
	ALTER TABLE Users
	ALTER COLUMN IsActive BIT NOT NULL
END



--创建表ConsumedServiceStatus，及关系
IF NOT EXISTS (SELECT TOP 1 1 FROM sys.tables WHERE name = 'ConsumedServiceStatus')
BEGIN
	CREATE TABLE ConsumedServiceStatus
	(
		ConsumedServiceStatusId INT NOT NULL,
		ConsumedServiceStatusStatus CHAR(40) NOT NULL,
		CONSTRAINT PK_ConsumedServiceStatus PRIMARY KEY CLUSTERED 
		(
			ConsumedServiceStatusId ASC
		)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
	) ON [PRIMARY]
	
	EXEC sp_executesql N'INSERT INTO ConsumedServiceStatus
	VALUES (1,''Consumed''),(2,''UserConfirmed'')'
END

IF NOT EXISTS (SELECT TOP 1 1 FROM sys.foreign_keys  where name='FK_ConsumedServices_ConsumedServiceStatus_ConsumedServiceStatusId')
BEGIN
	ALTER TABLE ConsumedServices  WITH CHECK ADD CONSTRAINT FK_ConsumedServices_ConsumedServiceStatus_ConsumedServiceStatusId FOREIGN KEY(ConsumedServiceStatusId)
	REFERENCES ConsumedServiceStatus (ConsumedServiceStatusId)
	
	ALTER TABLE ConsumedServices CHECK CONSTRAINT FK_ConsumedServices_ConsumedServiceStatus_ConsumedServiceStatusId
END
--创建表ConsumedServiceStatus，及关系



--创建消费二维码相关字段
IF NOT EXISTS (SELECT TOP 1 1 FROM sys.columns WHERE name = 'ConsumeCode' AND object_id=object_id('Users'))
BEGIN
	ALTER TABLE Users
	ADD ConsumeCode VARCHAR(36) NULL
END

IF NOT EXISTS (SELECT TOP 1 1 FROM sys.columns WHERE name = 'ConsumeCodeExpiredDate' AND object_id=object_id('Users'))
BEGIN
	ALTER TABLE Users
	ADD ConsumeCodeExpiredDate DATETIME NULL
END
--创建消费二维码相关字段



--创建表ConsumedServiceDetails，及相关外键
IF NOT EXISTS (SELECT TOP 1 1 FROM sys.tables WHERE name = 'ConsumedServiceDetails')
BEGIN
	CREATE TABLE ConsumedServiceDetails
	(
		ConsumedServiceId INT NOT NULL,
		PurchasedServiceId INT NOT NULL,
		[Time] TINYINT NOT NULL
		CONSTRAINT PK_ConsumedServiceDetails PRIMARY KEY CLUSTERED 
		(	ConsumedServiceId ASC,
			PurchasedServiceId ASC
		)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
	) ON [PRIMARY]
END

IF NOT EXISTS (SELECT TOP 1 1 FROM sys.foreign_keys  where name='FK_ConsumedServiceDetails_ConsumedServices_ConsumedServiceId')
BEGIN
	ALTER TABLE ConsumedServiceDetails  WITH CHECK ADD CONSTRAINT FK_ConsumedServiceDetails_ConsumedServices_ConsumedServiceId FOREIGN KEY(ConsumedServiceId)
	REFERENCES ConsumedServices (ConsumedServiceId)
	
	ALTER TABLE ConsumedServiceDetails CHECK CONSTRAINT FK_ConsumedServiceDetails_ConsumedServices_ConsumedServiceId
END

IF NOT EXISTS (SELECT TOP 1 1 FROM sys.foreign_keys  where name='FK_ConsumedServiceDetails_PurchasedServices_PurchasedServiceId')
BEGIN
	ALTER TABLE ConsumedServiceDetails  WITH CHECK ADD CONSTRAINT FK_ConsumedServiceDetails_PurchasedServices_PurchasedServiceId FOREIGN KEY(PurchasedServiceId)
	REFERENCES PurchasedServices (PurchasedServiceId)
	
	ALTER TABLE ConsumedServiceDetails CHECK CONSTRAINT FK_ConsumedServiceDetails_PurchasedServices_PurchasedServiceId
END
--创建表ConsumedServiceDetails，及相关外键


--用户表加备注字段
IF NOT EXISTS (SELECT TOP 1 1 FROM sys.columns WHERE name = 'Memo' AND object_id=object_id('Users'))
BEGIN
	ALTER TABLE Users
	ADD Memo NVARCHAR(500) NULL
END
--用户表加备注字段