IF NOT EXISTS (SELECT TOP 1 1 FROM sys.columns WHERE name = 'OncePriceOnSale' AND object_id=object_id('Services'))
BEGIN
	ALTER TABLE Services
	ADD OncePriceOnSale decimal(9, 2)
END

IF NOT EXISTS (SELECT TOP 1 1 FROM sys.columns WHERE name = 'TreatmentPriceOnSale' AND object_id=object_id('Services'))
BEGIN
	ALTER TABLE Services
	ADD TreatmentPriceOnSale decimal(9, 2)
END

IF NOT EXISTS (SELECT TOP 1 1 FROM sys.columns WHERE name = 'TreatmentTimeOnSale' AND object_id=object_id('Services'))
BEGIN
	ALTER TABLE Services
	ADD TreatmentTimeOnSale tinyint
END

IF NOT EXISTS (SELECT TOP 1 1 FROM sys.columns WHERE name = 'OncePriceOnSale' AND object_id=object_id('ServiceSnapShots'))
BEGIN
	ALTER TABLE ServiceSnapShots
	ADD OncePriceOnSale decimal(9, 2)
END

IF NOT EXISTS (SELECT TOP 1 1 FROM sys.columns WHERE name = 'TreatmentPriceOnSale' AND object_id=object_id('ServiceSnapShots'))
BEGIN
	ALTER TABLE ServiceSnapShots
	ADD TreatmentPriceOnSale decimal(9, 2)
END

IF NOT EXISTS (SELECT TOP 1 1 FROM sys.columns WHERE name = 'TreatmentTimeOnSale' AND object_id=object_id('ServiceSnapShots'))
BEGIN
	ALTER TABLE ServiceSnapShots
	ADD TreatmentTimeOnSale tinyint
END

declare @Tags table
(
	TagName nvarchar(5),
	ServiceTypeId int,
	Seq INT
)
INSERT INTO @Tags
(TagName,ServiceTypeId,Seq)
values
('补水',1,1),
('深层补水',1,2),
('锁水',1,3),
('滋养柔肤',1,4),
('收缩毛孔',1,5),
('嫩肤',1,6),
('水油平衡',1,7),
('美白',2,1),
('祛斑',2,2),
('提亮',2,3),
('祛黄',2,4),
('褪黑',2,5),
('嫩肤',2,6),
('补水',2,7),
('治疗闭口',3,1),
('治疗痤疮',3,2),
('祛痘祛印',3,3),
('补水保湿',3,4),
('提亮肤色',3,5),
('美白',3,6),
('提拉',4,1),
('紧致',4,2),
('改善干纹',4,3),
('改善细纹',4,4),
('减轻法令纹',4,5),
('补水保湿',4,6),
('提亮肤色',4,7),
('排毒减肥',5,1),
('减脂塑身',5,2),
('祛湿驱寒',5,3),
('疏通筋络',5,4),
('收敛毛孔',6,1),
('脱毛',6,2),
('补水保湿',6,3),
('提亮肤色',6,4),
('排毒',7,1),
('排湿',7,2),
('疏通筋络',7,3),
('改善体质',7,4),
('延缓衰老',7,5),
('提亮肤色',7,6),
('减肥塑身',7,7),
('祛湿驱寒',7,8),
('暖宫',7,9),
('半永久',8,1),
('彩妆',8,2),
('美妆',8,3),
('补水',9,1),
('锁水',9,2),
('抗敏脱敏',9,3),
('祛红血丝',9,4),
('祛激素',9,5),
('排毒',9,6)


IF NOT EXISTS(SELECT TOP 1 1 FROM sys.tables WHERE object_id=object_id('ServiceTypeTags'))
BEGIN
	CREATE TABLE [dbo].ServiceTypeTags(
		TagName NVARCHAR(5) NOT NULL,
		ServiceTypeId INT NOT NULL,
	 CONSTRAINT [PK_ServiceTypeTags] PRIMARY KEY CLUSTERED 
	(
		TagName ASC,
		ServiceTypeId ASC
	)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
	) ON [PRIMARY]
	
	
	INSERT INTO ServiceTypeTags
	(TagName,ServiceTypeId)
	SELECT TagName,ServiceTypeId FROM @Tags
END

IF NOT EXISTS (SELECT TOP 1 1 FROM sys.foreign_keys  where name='FK_ServiceTypeTags_ServiceTypes_ServiceTypeId')
BEGIN
	ALTER TABLE ServiceTypeTags  WITH CHECK ADD CONSTRAINT FK_ServiceTypeTags_ServiceTypes_ServiceTypeId FOREIGN KEY(ServiceTypeId)
	REFERENCES ServiceTypes(ServiceTypeId)
	
	ALTER TABLE ServiceTypeTags CHECK CONSTRAINT FK_ServiceTypeTags_ServiceTypes_ServiceTypeId
END

IF NOT EXISTS(SELECT TOP 1 1 FROM sys.tables WHERE object_id=object_id('ServiceFunctionalityTags'))
BEGIN
	CREATE TABLE [dbo].[ServiceFunctionalityTags](
	[TagName] [nvarchar](5) NOT NULL,
	[ServiceId] [int] NOT NULL,
	[Seq] [int] NOT NULL,
	 CONSTRAINT [PK_ServiceFunctionalityTags] PRIMARY KEY CLUSTERED 
	(
		[TagName] ASC,
		[ServiceId] ASC
	)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY],
	 CONSTRAINT [IX_ServiceFunctionalityTags_ServiceId_Seq] UNIQUE NONCLUSTERED 
	(
		[ServiceId] ASC,
		[Seq] ASC
	)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
	) ON [PRIMARY]
	
	INSERT INTO ServiceFunctionalityTags
	(TagName, ServiceId, Seq)
	SELECT t.TagName, s.ServiceId, t.Seq FROM [Services] s
	INNER JOIN @Tags t
	ON s.ServiceTypeId = t.ServiceTypeId AND t.Seq=1
END

IF NOT EXISTS (SELECT TOP 1 1 FROM sys.foreign_keys  where name='FK_ServiceFunctionalityTags_Services_ServiceId')
BEGIN
	ALTER TABLE ServiceFunctionalityTags  WITH CHECK ADD CONSTRAINT FK_ServiceFunctionalityTags_Services_ServiceId FOREIGN KEY(ServiceId)
	REFERENCES [Services](ServiceId)
	
	ALTER TABLE ServiceFunctionalityTags CHECK CONSTRAINT FK_ServiceFunctionalityTags_Services_ServiceId
END


IF NOT EXISTS(SELECT TOP 1 1 FROM sys.tables WHERE object_id=object_id('ServiceFunctionalityTagSnapShots'))
BEGIN
	CREATE TABLE [dbo].[ServiceFunctionalityTagSnapShots](
	[TagName] [nvarchar](5) NOT NULL,
	[ServiceSnapShotId] [int] NOT NULL,
	[Seq] [int] NOT NULL,
	 CONSTRAINT [PK_ServiceFunctionalityTagSnapShots] PRIMARY KEY CLUSTERED 
	(
		[TagName] ASC,
		[ServiceSnapShotId] ASC
	)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY],
	 CONSTRAINT [IX_ServiceFunctionalityTagSnapShots_ServiceSnapShotId_Seq] UNIQUE NONCLUSTERED 
	(
		[ServiceSnapShotId] ASC,
		[Seq] ASC
	)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
	) ON [PRIMARY]
	
	INSERT INTO ServiceFunctionalityTagSnapShots
	(TagName, ServiceSnapShotId, Seq)
	SELECT t.TagName, s.ServiceSnapShotId, t.Seq FROM ServiceSnapShots s
	INNER JOIN @Tags t
	ON s.ServiceTypeId = t.ServiceTypeId AND t.Seq=1
END
GO

IF NOT EXISTS (SELECT TOP 1 1 FROM sys.foreign_keys  where name='FK_ServiceFunctionalityTagSnapShots_ServiceSnapShots_ServiceSnapShotId')
BEGIN
	ALTER TABLE ServiceFunctionalityTagSnapShots  WITH CHECK ADD CONSTRAINT FK_ServiceFunctionalityTagSnapShots_ServiceSnapShots_ServiceSnapShotId FOREIGN KEY(ServiceSnapShotId)
	REFERENCES ServiceSnapShots(ServiceSnapShotId)
	
	ALTER TABLE ServiceFunctionalityTagSnapShots CHECK CONSTRAINT FK_ServiceFunctionalityTagSnapShots_ServiceSnapShots_ServiceSnapShotId
END

IF EXISTS (SELECT TOP 1 1 FROM sys.columns WHERE name='Functionality' AND object_id=object_id('Services'))
BEGIN
	ALTER TABLE [Services]
	DROP COLUMN Functionality
END

IF EXISTS (SELECT TOP 1 1 FROM sys.columns WHERE name='Functionality' AND object_id=object_id('ServiceSnapShots'))
BEGIN
	ALTER TABLE ServiceSnapShots
	DROP COLUMN Functionality
END


--1. 根据分类获取PredefinedTags
--2. 保存tag进Tags使用表
--3. 根据服务功效获取tags
--4. 根据tag获取相关的数据
