-------------------------------------------------
-- WYRAŹNIE: jeśli możesz, najpierw wyczyść dane
-------------------------------------------------
DELETE FROM Tickets;
DELETE FROM Screenings;
DELETE FROM Movies;
DELETE FROM Halls;

-------------------------------------------------
-- SALE (bez Id)
-------------------------------------------------
INSERT INTO Halls (Name, Capacity, Is3D) VALUES
('Sala 1', 120, 1),
('Sala 2', 80, 0),
('Sala VIP', 40, 1);

-------------------------------------------------
-- FILMY (bez Id)
-------------------------------------------------
INSERT INTO Movies (Title, DurationMin, Description, ReleaseDate, BasePrice) VALUES
('Incepcja', 148, 'Thriller sci-fi w reżyserii Christophera Nolana.', '2010-08-13', 25),
('Avatar 2', 190, 'Kontynuacja hitu Jamesa Camerona.', '2022-12-16', 30),
('Dune: Part Two', 165, 'Epickie sci-fi.', '2024-03-01', 28);

-------------------------------------------------
-- SEANSE (FK pobieramy po nazwach)
-------------------------------------------------
DECLARE @Hall1 INT = (SELECT TOP 1 Id FROM Halls WHERE Name = 'Sala 1');
DECLARE @Hall2 INT = (SELECT TOP 1 Id FROM Halls WHERE Name = 'Sala 2');
DECLARE @HallVip INT = (SELECT TOP 1 Id FROM Halls WHERE Name = 'Sala VIP');

DECLARE @MovieIncepcja INT = (SELECT TOP 1 Id FROM Movies WHERE Title = 'Incepcja');
DECLARE @MovieAvatar   INT = (SELECT TOP 1 Id FROM Movies WHERE Title = 'Avatar 2');
DECLARE @MovieDune     INT = (SELECT TOP 1 Id FROM Movies WHERE Title = 'Dune: Part Two');

INSERT INTO Screenings (MovieId, HallId, StartTime, Language, Is3D) VALUES
(@MovieIncepcja, @Hall1,   '2025-01-15 18:00:00', 'PL', 0),
(@MovieIncepcja, @Hall2,   '2025-01-16 20:30:00', 'EN', 0),
(@MovieAvatar,   @Hall1,   '2025-01-17 19:00:00', 'PL', 1),
(@MovieDune,     @HallVip, '2025-01-18 21:00:00', 'PL', 1);

-------------------------------------------------
-- BILETY (też bez Id, poprawiona składnia CTE)
-------------------------------------------------
DECLARE @Screening1 INT;
DECLARE @Screening3 INT;

;WITH OrderedScreenings AS
(
    SELECT Id,
           ROW_NUMBER() OVER (ORDER BY Id) AS rn
    FROM Screenings
)
SELECT @Screening1 = Id FROM OrderedScreenings WHERE rn = 1;

;WITH OrderedScreenings AS
(
    SELECT Id,
           ROW_NUMBER() OVER (ORDER BY Id) AS rn
    FROM Screenings
)
SELECT @Screening3 = Id FROM OrderedScreenings WHERE rn = 3;

INSERT INTO Tickets (ScreeningId, SeatNumber, Price, SoldAt) VALUES
(@Screening1, 'A1', 25, '2025-01-10 12:00:00'),
(@Screening1, 'A2', 25, '2025-01-10 12:05:00'),
(@Screening3, 'B5', 30, '2025-01-11 15:30:00');
