CREATE TABLE IF NOT EXISTS Orders (
    Id SERIAL PRIMARY KEY,
    ClientId VARCHAR(50) NOT NULL,
    ClientIp VARCHAR(50) NULL,
    DepartmentAddress VARCHAR(255) NOT NULL,
    Amount DECIMAL(18, 2) NOT NULL,
    Currency VARCHAR(10) NOT NULL,
    Status int DEFAULT 0 NOT NULL
);

CREATE OR REPLACE FUNCTION insert_order(
    client_id VARCHAR, 
    department_address VARCHAR, 
    amount DECIMAL, 
    currency VARCHAR,
    client_ip VARCHAR DEFAULT NULL,
    status INT DEFAULT 0
)
RETURNS TABLE(request_id INT) AS
$$
BEGIN
    INSERT INTO Orders(ClientId, DepartmentAddress, Amount, Currency, Status, ClientIp)
    VALUES (client_id, department_address, amount, currency, status, client_ip)
    RETURNING Id INTO request_id;
    RETURN NEXT;
END;
$$ LANGUAGE plpgsql;


CREATE OR REPLACE FUNCTION get_orders(
    client_id VARCHAR, 
    department_address VARCHAR
)
RETURNS TABLE(order_id INT, amount DECIMAL, currency VARCHAR, status INT, client_ip VARCHAR, department_address VARCHAR, client_id VARCHAR ) AS
$$
BEGIN
    RETURN QUERY
    SELECT Id, Amount, Currency, Status, ClientIp, DepartmentAddress, ClientId
    FROM Orders
    WHERE ClientId = client_id AND DepartmentAddress = department_address;
END;
$$ LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION get_order_by_id(
    order_id INT
)
RETURNS TABLE(order_id INT, client_id VARCHAR, department_address VARCHAR, amount DECIMAL, currency VARCHAR, status INT, client_ip VARCHAR) AS
$$
BEGIN
    RETURN QUERY
    SELECT Id, ClientId, DepartmentAddress, Amount, Currency, Status, ClientIp
    FROM Orders
    WHERE Id = order_id;
END;
$$ LANGUAGE plpgsql;