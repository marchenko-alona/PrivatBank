CREATE TABLE IF NOT EXISTS Orders (
    Id SERIAL PRIMARY KEY,
    ClientId VARCHAR(50),
    DepartmentAddress VARCHAR(255),
    Amount DECIMAL(18, 2),
    Currency VARCHAR(10),
    Status VARCHAR(50)
);
