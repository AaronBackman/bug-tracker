CREATE TABLE Users (
    Username varchar(255) PRIMARY KEY,
    Nickname varchar(255)
);

CREATE TABLE Projects (
    ProjectId int PRIMARY KEY,
    ProjectName varchar(255) NOT NULL
);

CREATE TABLE ProjectMembers (
    ProjectId int NOT NULL FOREIGN KEY REFERENCES Projects(ProjectId) ON DELETE CASCADE,
    Username varchar(255) NOT NULL FOREIGN KEY REFERENCES Users(Username) ON DELETE CASCADE,
    ProjectRole int NOT NULL,
    PRIMARY KEY(ProjectId, Username)
);

CREATE TABLE Tickets (
    Creator varchar(255) NOT NULL FOREIGN KEY REFERENCES Users(Username),
    DateCreated varchar(255) NOT NULL,
    Editor varchar(255) FOREIGN KEY REFERENCES Users(Username),
    DateEdited varchar(255),
    TicketName varchar(255) NOT NULL,
    TicketDescription varchar(255) NOT NULL,
    TicketPriority int NOT NULL,
    TicketCompleted bit NOT NULL,
    TicketId int PRIMARY KEY,
    ProjectId int NOT NULL FOREIGN KEY REFERENCES Projects(ProjectId) ON DELETE CASCADE
);

CREATE TABLE TicketHistories (
    TicketHistoryId int PRIMARY KEY,
    TicketId int NOT NULL FOREIGN KEY REFERENCES Tickets(TicketId),
    DateEdited varchar(255) NOT NULL,
    Editor varchar(255) NOT NULL FOREIGN KEY REFERENCES Users(Username),
    Change varchar(255) NOT NULL
);


// one lines
CREATE TABLE Users (Username varchar(255) PRIMARY KEY, Nickname varchar(255));

CREATE TABLE Projects (ProjectId int PRIMARY KEY, ProjectName varchar(255) NOT NULL);

CREATE TABLE ProjectMembers (ProjectId int NOT NULL FOREIGN KEY REFERENCES Projects(ProjectId) ON DELETE CASCADE, Username varchar(255) NOT NULL FOREIGN KEY REFERENCES Users(Username) ON DELETE CASCADE, ProjectRole int NOT NULL, PRIMARY KEY(ProjectId, Username));

CREATE TABLE Tickets (Creator varchar(255) NOT NULL FOREIGN KEY REFERENCES Users(Username),DateCreated varchar(255) NOT NULL,Editor varchar(255) FOREIGN KEY REFERENCES Users(Username),DateEdited varchar(255),TicketName varchar(255) NOT NULL,TicketDescription varchar(255) NOT NULL,TicketPriority int NOT NULL,TicketCompleted bit NOT NULL,TicketId int PRIMARY KEY,ProjectId int NOT NULL FOREIGN KEY REFERENCES Projects(ProjectId) ON DELETE CASCADE);

CREATE TABLE TicketHistories (TicketHistoryId int PRIMARY KEY,TicketId int NOT NULL FOREIGN KEY REFERENCES Tickets(TicketId),DateEdited varchar(255) NOT NULL,Editor varchar(255) NOT NULL FOREIGN KEY REFERENCES Users(Username),Change varchar(255) NOT NULL);
