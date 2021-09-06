CREATE TABLE Users (
    Id int IDENTITY(1,1) PRIMARY KEY,
    Nickname varchar(255),
    Email varchar(255) NOT NULL UNIQUE
);

CREATE TABLE Projects (
    Id int IDENTITY(1,1) PRIMARY KEY,
    ProjectName varchar(255) NOT NULL,
    ProjectGUID UNIQUEIDENTIFIER NOT NULL UNIQUE DEFAULT NEWID(),
    OwnerId int NOT NULL FOREIGN KEY REFERENCES Users(Id)
);

CREATE TABLE ProjectMembers (
    ProjectId int NOT NULL FOREIGN KEY REFERENCES Projects(Id) ON DELETE CASCADE,
    UserId int NOT NULL FOREIGN KEY REFERENCES Users(Id) ON DELETE CASCADE,
    ProjectRole int NOT NULL,
    Id int IDENTITY(1,1) PRIMARY KEY,
    ProjectMemberGUID UNIQUEIDENTIFIER NOT NULL UNIQUE DEFAULT NEWID(),
    Unique(ProjectId, UserId)
);

CREATE TABLE Tickets (
    Id int IDENTITY(1,1) PRIMARY KEY,
    TicketGUID UNIQUEIDENTIFIER NOT NULL UNIQUE DEFAULT NEWID(),
    CreatorId int NOT NULL FOREIGN KEY REFERENCES Users(Id),
    DateCreated varchar(255) NOT NULL,
    TicketName varchar(255) NOT NULL,
    TicketDescription varchar(255) NOT NULL,
    TicketPriority int NOT NULL,
    TicketCompleted bit NOT NULL,
    ProjectId int NOT NULL FOREIGN KEY REFERENCES Projects(Id) ON DELETE CASCADE
    AssignedToId int NOT NULL FOREIGN KEY REFERENCES Users(Id)
);

CREATE TABLE TicketHistories (
    Id int IDENTITY(1,1) PRIMARY KEY,
    TicketId int NOT NULL FOREIGN KEY REFERENCES Tickets(Id) ON DELETE CASCADE,
    DateEdited varchar(255) NOT NULL,
    EditorId int NOT NULL FOREIGN KEY REFERENCES Users(Id),
    Change varchar(255) NOT NULL
);


// one lines
