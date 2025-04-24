UPDATE Salons SET [Description]='店铺描述' WHERE [Description] IS NULL OR [Description]=''

ALTER TABLE Salons
ALTER COLUMN [Description] nvarchar(200) NOT NULL


UPDATE [Services] SET Functionality='美容功效' WHERE Functionality is null or Functionality=''

ALTER TABLE [Services]
ALTER COLUMN [Functionality] nvarchar(200) NOT NULL