USE [C:\USERS\VIGAN\DOCUMENTS\VISUAL STUDIO 2015\PROJECTS\DMS\MODEL\DB CONTEXT\DMSDB.MDF];

CREATE TABLE Users
(
	UserID INT PRIMARY KEY IDENTITY(2342534, 1),
	Username VARCHAR(50) NOT NULL UNIQUE,
	Email VARCHAR(255) NOT NULL UNIQUE,
	Password CHAR(40) NOT NULL,
	Salt CHAR(6) NOT NULL,
	Active BIT NOT NULL DEFAULT 1,
	StorageSize INT NOT NULL DEFAULT 15,
	Type CHAR(1) NOT NULL DEFAULT 'U'-- A, U
);

CREATE TABLE Files
(
	FileID INT PRIMARY KEY IDENTITY(1, 1),
	UserID INT,
	Title NVARCHAR(MAX) NOT NULL,
	ShortDesc TEXT NOT NULL,
	RelativeDirectory TEXT NOT NULL, 
	AccessLevel CHAR(1) NOT NULL DEFAULT 'N',-- N, P, L
	LastVersion INT NOT NULL DEFAULT 1,
	Keywords VARCHAR(MAX) NOT NULL,
	LastModified DATETIME NOT NULL,
	ExternalLink VARCHAR(255) NOT NULL UNIQUE,
	FOREIGN KEY (UserID) REFERENCES Users(UserID) ON DELETE SET NULL ON UPDATE CASCADE 
);

CREATE TABLE FileVersion
(
	VerNo INT NOT NULL,
	Name TEXT NOT NULL,
	FileID INT NOT NULL,
	FOREIGN KEY (FileID) REFERENCES Files (FileID) ON UPDATE CASCADE ON DELETE CASCADE,
	PRIMARY KEY(FileID, VerNo)
);

CREATE TABLE FileShare
(
	UserID INT NOT NULL,
	FileID INT NOT NULL,
	PRIMARY KEY(UserID, FileID),
	FOREIGN KEY (FileID) REFERENCES Files (FileID) ON DELETE CASCADE ON UPDATE CASCADE,
	FOREIGN KEY (UserID) REFERENCES Users (UserID) ON DELETE NO ACTION ON UPDATE NO ACTION
);

CREATE Table AccessRequest
(
	RequestID INT NOT NULL PRIMARY KEY IDENTITY (1,1),
	UserID INT NOT NULL,
	FileID INT NOT NULL,
	Status CHAR(1) NOT NULL DEFAULT 'P',-- A, P, D
	Stamp DATETIME NOT NULL,
	FOREIGN KEY (FileID) REFERENCES Files (FileID) ON UPDATE CASCADE ON DELETE CASCADE,
	FOREIGN KEY (UserID) REFERENCES Users (UserID) ON UPDATE NO ACTION ON DELETE NO ACTION
);

CREATE Table SizeRequest
(
	RequestID INT NOT NULL PRIMARY KEY IDENTITY (1,1),
	UserID INT NOT NULL,
	Amount INT NOT NULL,
	Status CHAR(1) NOT NULL DEFAULT 'P',-- A, P, D
	Stamp DATETIME NOT NULL,
	FOREIGN KEY (UserID) REFERENCES Users (UserID) ON UPDATE CASCADE ON DELETE CASCADE 
);

GO
CREATE TRIGGER trgDelUser 
	ON Users
	INSTEAD OF DELETE
	AS
BEGIN
	DELETE FROM FileShare 
	WHERE UserID IN (SELECT d.UserID from DELETED d);
	DELETE FROM AccessRequest 
	WHERE UserID IN (SELECT d.UserID from DELETED d);
	DELETE FROM Users 
	WHERE UserID IN (SELECT d.UserID from DELETED d);
END
GO

CREATE FUNCTION [dbo].[Split]
(
    @String NVARCHAR(4000),
    @Delimiter NCHAR(1)
)
RETURNS TABLE
AS
RETURN
(
    WITH Split(stpos,endpos)
    AS(
        SELECT 0 AS stpos, CHARINDEX(@Delimiter,@String) AS endpos
        UNION ALL
        SELECT endpos+1, CHARINDEX(@Delimiter,@String,endpos+1)
            FROM Split
            WHERE endpos > 0
    )
    SELECT 'Id' = ROW_NUMBER() OVER (ORDER BY (SELECT 1)),
        'Data' = SUBSTRING(@String,stpos,COALESCE(NULLIF(endpos,0),LEN(@String)+1)-stpos)
    FROM Split
)
GO

GO
CREATE PROCEDURE spAtLeastOnceKeyword @Keywords NVARCHAR(MAX)
AS
BEGIN
DECLARE @SQL NVARCHAR(MAX);
SET @Keywords = REPLACE(@Keywords, ', ', ',');
SET @Keywords = REPLACE(@Keywords, ' ,', ',');
SET @Keywords = REPLACE(@Keywords, ' , ', ',');
SET @Keywords = REPLACE(@Keywords, ' ,', ',');
SET @Keywords = REPLACE(@Keywords, ''' ', '''');
SET @Keywords = REPLACE(@Keywords, ' '' ', '''');
SET @Keywords = REPLACE(@Keywords, ' ''', '''');

SET @SQL = 'SELECT TOP 100 * FROM (
SELECT distinct f.FileID, f.UserID, u.Username, f.Title, f.LastVersion, f.LastModified,
Versions = STUFF((
	SELECT '','' + CONVERT(VARCHAR, v2.Verno)
	FROM FileVersion v2
	WHERE v.FileID = v2.FileID
    FOR XML PATH(''''), TYPE).value(''.'', ''NVARCHAR(MAX)''), 1, 1, ''''),
KeywordCount = (SELECT COUNT(*) FROM Split(RTRIM(LTRIM(REPLACE(REPLACE(f.Keywords, '' ,'', '',''), '', '', '',''))) , '','') AS d WHERE d.Data IN (' + @Keywords +'))
FROM Files f INNER JOIN FileVersion v ON f.FileID = v.FileID INNER JOIN Users u ON f.UserID = u.UserID
WHERE f.AccessLevel != ''N''
) AS T
WHERE t.KeywordCount > 0
ORDER BY T.KeywordCount DESC;';
EXECUTE(@SQL);
END
GO