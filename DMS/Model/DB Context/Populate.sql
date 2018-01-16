USE [C:\USERS\VIGAN\DOCUMENTS\VISUAL STUDIO 2015\PROJECTS\DMS\MODEL\DB CONTEXT\DMSDB.MDF];

GO
SET IDENTITY_INSERT Users ON;
INSERT INTO Users (UserID, Username, Email, Password, Salt, Active, StorageSize, Type, ExternalLink) VALUES 
(2342534, 'admin', 'php.test.130795@gmail.com', '0CF5927396B1D73EDA9F400AACBDD4BB3ECD0AD2', '651768', 1, 0, 'A');
INSERT INTO Users (UserID, Username, Email, Password, Salt, Active, StorageSize, Type, ExternalLink) VALUES 
(2342535, 'user1', 'vigan.abd@gmail.com', '27792D6590AA43C01D5A796AFF8F3FDFDB7A3607', '107764', 1, 15, 'U');
INSERT INTO Users (UserID, Username, Email, Password, Salt, Active, StorageSize, Type, ExternalLink) VALUES 
(2342536, 'user2', 'user2@mail.com', '27792D6590AA43C01D5A796AFF8F3FDFDB7A3607', '107764', 1, 15, 'U');
INSERT INTO Users (UserID, Username, Email, Password, Salt, Active, StorageSize, Type, ExternalLink) VALUES 
(2342537, 'user3', 'user3@mail.com', '27792D6590AA43C01D5A796AFF8F3FDFDB7A3607', '107764', 1, 15, 'U');
SET IDENTITY_INSERT Users OFF;

SET IDENTITY_INSERT Files ON;
INSERT INTO Files (FileID, UserID, Title, ShortDesc, RelativeDirectory, AccessLevel, LastVersion, Keywords, LastModified) VALUES
(1, 2342535, 'Sunny beach picture', 'Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industrys standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum.', '/user1', 'P', 2, 'sunny,beach,picture,document,sun', '2016-07-07 23:22:11','EBE26A9DCFA4DE3616731C84F262C2D065E62BEA192531568');

INSERT INTO Files (FileID, UserID, Title, ShortDesc, RelativeDirectory, AccessLevel, LastVersion, Keywords, LastModified) VALUES
(2, 2342535, 'PHP and MySQL Web Development', 'Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industrys standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum.', '/user1', 'L', 1, 'php,mysql,web,development,book,programming,coding,website,database', '2016-07-07 23:22:11','267C09B034CBBF11BD6F0B1C6EBBCA9B4AC0AAEB211794569');
INSERT INTO Files (FileID, UserID, Title, ShortDesc, RelativeDirectory, AccessLevel, LastVersion, Keywords, LastModified) VALUES
(3, 2342535, 'PHP Development', 'Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industrys standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum.', '/user1', 'N', 1, 'php,web,development,book,programming,coding,website', '2016-07-07 23:22:11','D8AA701B96957BAE4D157254B596DEF3F41E1849211794569');
INSERT INTO Files (FileID, UserID, Title, ShortDesc, RelativeDirectory, AccessLevel, LastVersion, Keywords, LastModified) VALUES
(7, 2342536, 'PHP Development', 'My testing', '/user2', 'P', 1, 'test,testing', '2016-07-07 23:22:11','E65C71EA689E780D2F2264BB503AC4E2D4FC0C44211794569');
SET IDENTITY_INSERT Files OFF;

INSERT INTO FileVersion (VerNo, Name, FileID) VALUES
(1, 'Sunny beach picture-v1.txt', 1);
INSERT INTO FileVersion (VerNo, Name, FileID) VALUES
(2, 'Sunny beach picture-v2.txt', 1);
INSERT INTO FileVersion (VerNo, Name, FileID) VALUES
(1, 'PHP and MySQL Web Development-v1.txt', 2);
INSERT INTO FileVersion (VerNo, Name, FileID) VALUES
(1, 'PHP Development-v1.txt', 3);
INSERT INTO FileVersion (VerNo, Name, FileID) VALUES
(1, 'Testing2-v1.png', 7);

INSERT INTO FileShare (UserID, FileID) VALUES
(2342537, 2);

INSERT INTO AccessRequest (UserID, FileID, Status, Stamp) VALUES
(2342536, 3, 'P', '2016-07-07 23:22:11');
INSERT INTO AccessRequest (UserID, FileID, Status, Stamp) VALUES
(2342536, 2, 'D', '2016-07-07 23:22:11');
INSERT INTO AccessRequest (UserID, FileID, Status, Stamp) VALUES
(2342536, 2, 'P', '2016-07-07 23:22:11');

INSERT INTO SizeRequest (UserID, Amount, Status, Stamp) VALUES
(2342536, 10, 'P', '2016-07-07 23:22:11');
INSERT INTO SizeRequest (UserID, Amount, Status, Stamp) VALUES
(2342537, 5, 'D', '2016-07-07 23:22:11');
INSERT INTO SizeRequest (UserID, Amount, Status, Stamp) VALUES
(2342535, 3, 'A', '2016-07-07 23:22:11');
INSERT INTO SizeRequest (UserID, Amount, Status, Stamp) VALUES
(2342535, 7, 'P', '2016-07-07 23:22:11');
INSERT INTO SizeRequest (UserID, Amount, Status, Stamp) VALUES
(2342535, 1, 'A', '12/11/2016 15:49:50');