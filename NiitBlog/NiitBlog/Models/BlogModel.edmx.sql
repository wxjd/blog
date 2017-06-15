
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, 2012 and Azure
-- --------------------------------------------------
-- Date Created: 06/13/2017 12:46:46
-- Generated from EDMX file: G:\NiitBlog\NiitBlog\NiitBlog\Models\BlogModel.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [Blog];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[FK_Albums_Users]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Albums] DROP CONSTRAINT [FK_Albums_Users];
GO
IF OBJECT_ID(N'[dbo].[FK_Articles_Categories]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Articles] DROP CONSTRAINT [FK_Articles_Categories];
GO
IF OBJECT_ID(N'[dbo].[FK_Articles_Users]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Articles] DROP CONSTRAINT [FK_Articles_Users];
GO
IF OBJECT_ID(N'[dbo].[FK_ArticleTagMapping_Articles]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[ArticleTagMapping] DROP CONSTRAINT [FK_ArticleTagMapping_Articles];
GO
IF OBJECT_ID(N'[dbo].[FK_ArticleTagMapping_Tags]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[ArticleTagMapping] DROP CONSTRAINT [FK_ArticleTagMapping_Tags];
GO
IF OBJECT_ID(N'[dbo].[FK_Categories_Users]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Categories] DROP CONSTRAINT [FK_Categories_Users];
GO
IF OBJECT_ID(N'[dbo].[FK_Comments_Articles]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Comments] DROP CONSTRAINT [FK_Comments_Articles];
GO
IF OBJECT_ID(N'[dbo].[FK_Comments_Photos]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Comments] DROP CONSTRAINT [FK_Comments_Photos];
GO
IF OBJECT_ID(N'[dbo].[FK_Comments_Users]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Comments] DROP CONSTRAINT [FK_Comments_Users];
GO
IF OBJECT_ID(N'[dbo].[FK_Photos_Albums]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Photos] DROP CONSTRAINT [FK_Photos_Albums];
GO
IF OBJECT_ID(N'[dbo].[FK_Photos_Users]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Photos] DROP CONSTRAINT [FK_Photos_Users];
GO
IF OBJECT_ID(N'[dbo].[FK_Tags_Users]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Tags] DROP CONSTRAINT [FK_Tags_Users];
GO
IF OBJECT_ID(N'[dbo].[FK_Users_UserRoles]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Users] DROP CONSTRAINT [FK_Users_UserRoles];
GO

-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[__MigrationHistory]', 'U') IS NOT NULL
    DROP TABLE [dbo].[__MigrationHistory];
GO
IF OBJECT_ID(N'[dbo].[Albums]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Albums];
GO
IF OBJECT_ID(N'[dbo].[Articles]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Articles];
GO
IF OBJECT_ID(N'[dbo].[ArticleTagMapping]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ArticleTagMapping];
GO
IF OBJECT_ID(N'[dbo].[Categories]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Categories];
GO
IF OBJECT_ID(N'[dbo].[Comments]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Comments];
GO
IF OBJECT_ID(N'[dbo].[Complaint]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Complaint];
GO
IF OBJECT_ID(N'[dbo].[Photos]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Photos];
GO
IF OBJECT_ID(N'[dbo].[Tags]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Tags];
GO
IF OBJECT_ID(N'[dbo].[UserRoles]', 'U') IS NOT NULL
    DROP TABLE [dbo].[UserRoles];
GO
IF OBJECT_ID(N'[dbo].[Users]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Users];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'C__MigrationHistory'
CREATE TABLE [dbo].[C__MigrationHistory] (
    [MigrationId] nvarchar(150)  NOT NULL,
    [ContextKey] nvarchar(300)  NOT NULL,
    [Model] varbinary(max)  NOT NULL,
    [ProductVersion] nvarchar(32)  NOT NULL
);
GO

-- Creating table 'Albums'
CREATE TABLE [dbo].[Albums] (
    [AlbumID] int IDENTITY(1,1) NOT NULL,
    [AlbumName] nvarchar(20)  NOT NULL,
    [Description] nvarchar(150)  NULL,
    [CoverPath] nvarchar(100)  NULL,
    [PhotoNum] int  NULL,
    [CreateTime] datetime  NULL,
    [UID] int  NOT NULL
);
GO

-- Creating table 'Articles'
CREATE TABLE [dbo].[Articles] (
    [AID] int IDENTITY(1,1) NOT NULL,
    [Title] nvarchar(200)  NOT NULL,
    [Content] nvarchar(max)  NOT NULL,
    [CreateTime] datetime  NOT NULL,
    [ViewNum] int  NULL,
    [CommentNum] int  NULL,
    [CID] int  NOT NULL,
    [UID] int  NOT NULL,
    [Summery] nvarchar(500)  NOT NULL
);
GO

-- Creating table 'ArticleTagMapping'
CREATE TABLE [dbo].[ArticleTagMapping] (
    [AID] int  NOT NULL,
    [TID] int  NOT NULL,
    [Description] nvarchar(100)  NULL,
    [ID] int IDENTITY(1,1) NOT NULL
);
GO

-- Creating table 'Categories'
CREATE TABLE [dbo].[Categories] (
    [CID] int IDENTITY(1,1) NOT NULL,
    [CName] nvarchar(50)  NOT NULL,
    [UID] int  NOT NULL
);
GO

-- Creating table 'Comments'
CREATE TABLE [dbo].[Comments] (
    [ID] int IDENTITY(1,1) NOT NULL,
    [Content] nvarchar(1000)  NOT NULL,
    [AID] int  NULL,
    [PID] int  NULL,
    [CreateTime] datetime  NOT NULL,
    [UID] int  NOT NULL,
    [ParentId] int  NULL
);
GO

-- Creating table 'Complaint'
CREATE TABLE [dbo].[Complaint] (
    [ID] int IDENTITY(1,1) NOT NULL,
    [title] nvarchar(150)  NOT NULL,
    [text] nvarchar(4000)  NOT NULL,
    [email] nvarchar(100)  NOT NULL
);
GO

-- Creating table 'Photos'
CREATE TABLE [dbo].[Photos] (
    [PID] int IDENTITY(1,1) NOT NULL,
    [PhotoName] nvarchar(50)  NOT NULL,
    [Description] nvarchar(150)  NULL,
    [Path] nvarchar(100)  NOT NULL,
    [Thumbnail] nvarchar(100)  NOT NULL,
    [CreateTime] datetime  NULL,
    [AlbumID] int  NOT NULL,
    [UID] int  NOT NULL,
    [Thumbnailw] nvarchar(100)  NOT NULL,
    [Thumbnailw_width] int  NULL,
    [Thumbnailw_height] int  NULL
);
GO

-- Creating table 'Tags'
CREATE TABLE [dbo].[Tags] (
    [TID] int IDENTITY(1,1) NOT NULL,
    [TName] nvarchar(50)  NOT NULL,
    [UID] int  NOT NULL
);
GO

-- Creating table 'UserRoles'
CREATE TABLE [dbo].[UserRoles] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(50)  NOT NULL
);
GO

-- Creating table 'Users'
CREATE TABLE [dbo].[Users] (
    [UID] int IDENTITY(1,1) NOT NULL,
    [UserName] nvarchar(20)  NOT NULL,
    [Password] nvarchar(300)  NOT NULL,
    [NickName] nvarchar(50)  NOT NULL,
    [Description] nvarchar(150)  NULL,
    [Gender] nvarchar(4)  NULL,
    [Email] nvarchar(100)  NOT NULL,
    [HeadPic] nvarchar(100)  NULL,
    [SelfIntro] nvarchar(150)  NULL,
    [RegTime] datetime  NOT NULL,
    [ActiveTime] datetime  NULL,
    [LastLoginTime] datetime  NULL,
    [Status] int  NULL,
    [Mid] nvarchar(50)  NULL,
    [UserRoleID] int  NOT NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [MigrationId], [ContextKey] in table 'C__MigrationHistory'
ALTER TABLE [dbo].[C__MigrationHistory]
ADD CONSTRAINT [PK_C__MigrationHistory]
    PRIMARY KEY CLUSTERED ([MigrationId], [ContextKey] ASC);
GO

-- Creating primary key on [AlbumID] in table 'Albums'
ALTER TABLE [dbo].[Albums]
ADD CONSTRAINT [PK_Albums]
    PRIMARY KEY CLUSTERED ([AlbumID] ASC);
GO

-- Creating primary key on [AID] in table 'Articles'
ALTER TABLE [dbo].[Articles]
ADD CONSTRAINT [PK_Articles]
    PRIMARY KEY CLUSTERED ([AID] ASC);
GO

-- Creating primary key on [ID] in table 'ArticleTagMapping'
ALTER TABLE [dbo].[ArticleTagMapping]
ADD CONSTRAINT [PK_ArticleTagMapping]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [CID] in table 'Categories'
ALTER TABLE [dbo].[Categories]
ADD CONSTRAINT [PK_Categories]
    PRIMARY KEY CLUSTERED ([CID] ASC);
GO

-- Creating primary key on [ID] in table 'Comments'
ALTER TABLE [dbo].[Comments]
ADD CONSTRAINT [PK_Comments]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [ID] in table 'Complaint'
ALTER TABLE [dbo].[Complaint]
ADD CONSTRAINT [PK_Complaint]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [PID] in table 'Photos'
ALTER TABLE [dbo].[Photos]
ADD CONSTRAINT [PK_Photos]
    PRIMARY KEY CLUSTERED ([PID] ASC);
GO

-- Creating primary key on [TID] in table 'Tags'
ALTER TABLE [dbo].[Tags]
ADD CONSTRAINT [PK_Tags]
    PRIMARY KEY CLUSTERED ([TID] ASC);
GO

-- Creating primary key on [Id] in table 'UserRoles'
ALTER TABLE [dbo].[UserRoles]
ADD CONSTRAINT [PK_UserRoles]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [UID] in table 'Users'
ALTER TABLE [dbo].[Users]
ADD CONSTRAINT [PK_Users]
    PRIMARY KEY CLUSTERED ([UID] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- Creating foreign key on [UID] in table 'Albums'
ALTER TABLE [dbo].[Albums]
ADD CONSTRAINT [FK_Albums_Users]
    FOREIGN KEY ([UID])
    REFERENCES [dbo].[Users]
        ([UID])
    ON DELETE CASCADE ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Albums_Users'
CREATE INDEX [IX_FK_Albums_Users]
ON [dbo].[Albums]
    ([UID]);
GO

-- Creating foreign key on [AlbumID] in table 'Photos'
ALTER TABLE [dbo].[Photos]
ADD CONSTRAINT [FK_Photos_Albums]
    FOREIGN KEY ([AlbumID])
    REFERENCES [dbo].[Albums]
        ([AlbumID])
    ON DELETE CASCADE ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Photos_Albums'
CREATE INDEX [IX_FK_Photos_Albums]
ON [dbo].[Photos]
    ([AlbumID]);
GO

-- Creating foreign key on [CID] in table 'Articles'
ALTER TABLE [dbo].[Articles]
ADD CONSTRAINT [FK_Articles_Categories]
    FOREIGN KEY ([CID])
    REFERENCES [dbo].[Categories]
        ([CID])
    ON DELETE CASCADE ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Articles_Categories'
CREATE INDEX [IX_FK_Articles_Categories]
ON [dbo].[Articles]
    ([CID]);
GO

-- Creating foreign key on [UID] in table 'Articles'
ALTER TABLE [dbo].[Articles]
ADD CONSTRAINT [FK_Articles_Users]
    FOREIGN KEY ([UID])
    REFERENCES [dbo].[Users]
        ([UID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Articles_Users'
CREATE INDEX [IX_FK_Articles_Users]
ON [dbo].[Articles]
    ([UID]);
GO

-- Creating foreign key on [AID] in table 'ArticleTagMapping'
ALTER TABLE [dbo].[ArticleTagMapping]
ADD CONSTRAINT [FK_ArticleTagMapping_Articles]
    FOREIGN KEY ([AID])
    REFERENCES [dbo].[Articles]
        ([AID])
    ON DELETE CASCADE ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_ArticleTagMapping_Articles'
CREATE INDEX [IX_FK_ArticleTagMapping_Articles]
ON [dbo].[ArticleTagMapping]
    ([AID]);
GO

-- Creating foreign key on [AID] in table 'Comments'
ALTER TABLE [dbo].[Comments]
ADD CONSTRAINT [FK_Comments_Articles]
    FOREIGN KEY ([AID])
    REFERENCES [dbo].[Articles]
        ([AID])
    ON DELETE CASCADE ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Comments_Articles'
CREATE INDEX [IX_FK_Comments_Articles]
ON [dbo].[Comments]
    ([AID]);
GO

-- Creating foreign key on [TID] in table 'ArticleTagMapping'
ALTER TABLE [dbo].[ArticleTagMapping]
ADD CONSTRAINT [FK_ArticleTagMapping_Tags]
    FOREIGN KEY ([TID])
    REFERENCES [dbo].[Tags]
        ([TID])
    ON DELETE CASCADE ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_ArticleTagMapping_Tags'
CREATE INDEX [IX_FK_ArticleTagMapping_Tags]
ON [dbo].[ArticleTagMapping]
    ([TID]);
GO

-- Creating foreign key on [UID] in table 'Categories'
ALTER TABLE [dbo].[Categories]
ADD CONSTRAINT [FK_Categories_Users]
    FOREIGN KEY ([UID])
    REFERENCES [dbo].[Users]
        ([UID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Categories_Users'
CREATE INDEX [IX_FK_Categories_Users]
ON [dbo].[Categories]
    ([UID]);
GO

-- Creating foreign key on [PID] in table 'Comments'
ALTER TABLE [dbo].[Comments]
ADD CONSTRAINT [FK_Comments_Photos]
    FOREIGN KEY ([PID])
    REFERENCES [dbo].[Photos]
        ([PID])
    ON DELETE CASCADE ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Comments_Photos'
CREATE INDEX [IX_FK_Comments_Photos]
ON [dbo].[Comments]
    ([PID]);
GO

-- Creating foreign key on [UID] in table 'Comments'
ALTER TABLE [dbo].[Comments]
ADD CONSTRAINT [FK_Comments_Users]
    FOREIGN KEY ([UID])
    REFERENCES [dbo].[Users]
        ([UID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Comments_Users'
CREATE INDEX [IX_FK_Comments_Users]
ON [dbo].[Comments]
    ([UID]);
GO

-- Creating foreign key on [UID] in table 'Photos'
ALTER TABLE [dbo].[Photos]
ADD CONSTRAINT [FK_Photos_Users]
    FOREIGN KEY ([UID])
    REFERENCES [dbo].[Users]
        ([UID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Photos_Users'
CREATE INDEX [IX_FK_Photos_Users]
ON [dbo].[Photos]
    ([UID]);
GO

-- Creating foreign key on [UID] in table 'Tags'
ALTER TABLE [dbo].[Tags]
ADD CONSTRAINT [FK_Tags_Users]
    FOREIGN KEY ([UID])
    REFERENCES [dbo].[Users]
        ([UID])
    ON DELETE CASCADE ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Tags_Users'
CREATE INDEX [IX_FK_Tags_Users]
ON [dbo].[Tags]
    ([UID]);
GO

-- Creating foreign key on [UserRoleID] in table 'Users'
ALTER TABLE [dbo].[Users]
ADD CONSTRAINT [FK_Users_UserRoles]
    FOREIGN KEY ([UserRoleID])
    REFERENCES [dbo].[UserRoles]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Users_UserRoles'
CREATE INDEX [IX_FK_Users_UserRoles]
ON [dbo].[Users]
    ([UserRoleID]);
GO

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------