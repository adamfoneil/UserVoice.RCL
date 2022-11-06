CREATE SCHEMA [uservoice]

GO

CREATE TABLE [uservoice].[AcceptanceRequest] (
    [Id] int identity(1,1)  NOT NULL,
    [ItemId] int   NOT NULL,
    [UserId] int   NOT NULL,
    [Response] int   NOT NULL,
    [Comments] nvarchar(max)   NULL,
    [DateCreated] datetime   NOT NULL,
    [CreatedBy] nvarchar(50)   NOT NULL,
    [DateModified] datetime   NULL,
    [ModifiedBy] nvarchar(50)   NULL,
    CONSTRAINT [PK_uservoiceAcceptanceRequest] PRIMARY KEY ([ItemId] ASC, [UserId] ASC),
    CONSTRAINT [U_uservoiceAcceptanceRequest_Id] UNIQUE ([Id] ASC)
)

GO

CREATE TABLE [uservoice].[Comment] (
    [Id] int identity(1,1)  NOT NULL,
    [ItemId] int   NOT NULL,
    [ItemStatus] int   NULL,
    [Body] nvarchar(max)   NOT NULL,
    [DateCreated] datetime   NOT NULL,
    [CreatedBy] nvarchar(50)   NOT NULL,
    [DateModified] datetime   NULL,
    [ModifiedBy] nvarchar(50)   NULL,
    CONSTRAINT [PK_uservoiceComment] PRIMARY KEY ([Id] ASC)
)

GO

CREATE TABLE [uservoice].[Item] (
    [Id] int identity(1,1)  NOT NULL,
    [Type] int   NOT NULL,
    [Title] nvarchar(255)   NOT NULL,
    [Body] nvarchar(max)   NULL,
    [StatusCommentId] int   NULL,
    [IsActive] bit   NOT NULL,
    [DateCreated] datetime   NOT NULL,
    [CreatedBy] nvarchar(50)   NOT NULL,
    [DateModified] datetime   NULL,
    [ModifiedBy] nvarchar(50)   NULL,
    CONSTRAINT [PK_uservoiceItem] PRIMARY KEY ([Id] ASC)
)

GO

CREATE TABLE [uservoice].[User] (
    [Id] int identity(1,1)  NOT NULL,
    [Name] nvarchar(50)   NOT NULL,
    [Email] nvarchar(50)   NULL,
    [Role] int   NOT NULL,
    [TimeZoneId] nvarchar(50)   NULL,
    [IsActive] bit   NOT NULL,
    [DateCreated] datetime   NOT NULL,
    [CreatedBy] nvarchar(50)   NOT NULL,
    [DateModified] datetime   NULL,
    [ModifiedBy] nvarchar(50)   NULL,
    CONSTRAINT [PK_uservoiceUser] PRIMARY KEY ([Name] ASC),
    CONSTRAINT [U_uservoiceUser_Email] UNIQUE ([Email] ASC),
    CONSTRAINT [U_uservoiceUser_Id] UNIQUE ([Id] ASC)
)

GO

CREATE TABLE [uservoice].[Vote] (
    [Id] int identity(1,1)  NOT NULL,
    [ItemId] int   NOT NULL,
    [UserId] int   NOT NULL,
    [Upvoted] bit   NOT NULL,
    [DateCreated] datetime   NOT NULL,
    [CreatedBy] nvarchar(50)   NOT NULL,
    [DateModified] datetime   NULL,
    [ModifiedBy] nvarchar(50)   NULL,
    CONSTRAINT [PK_uservoiceVote] PRIMARY KEY ([ItemId] ASC, [UserId] ASC),
    CONSTRAINT [U_uservoiceVote_Id] UNIQUE ([Id] ASC)
)

GO

ALTER TABLE [uservoice].[AcceptanceRequest] ADD CONSTRAINT [FK_uservoiceAcceptanceRequest_ItemId] FOREIGN KEY ([ItemId]) REFERENCES [uservoice].[Item] ([Id])

GO

ALTER TABLE [uservoice].[AcceptanceRequest] ADD CONSTRAINT [FK_uservoiceAcceptanceRequest_UserId] FOREIGN KEY ([UserId]) REFERENCES [uservoice].[User] ([Id])

GO

ALTER TABLE [uservoice].[Comment] ADD CONSTRAINT [FK_uservoiceComment_ItemId] FOREIGN KEY ([ItemId]) REFERENCES [uservoice].[Item] ([Id])

GO

ALTER TABLE [uservoice].[Item] ADD CONSTRAINT [FK_uservoiceItem_StatusCommentId] FOREIGN KEY ([StatusCommentId]) REFERENCES [uservoice].[Comment] ([Id])

GO

CREATE TYPE [uservoice].[IdList] AS TABLE (
    [Id] int NOT NULL PRIMARY KEY
)

GO