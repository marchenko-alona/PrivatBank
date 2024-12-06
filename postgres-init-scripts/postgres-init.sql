CREATE TABLE IF NOT EXISTS Orders (
    Id SERIAL PRIMARY KEY,
    ClientId VARCHAR(50) NOT NULL,
    ClientIp VARCHAR(50) NULL,
    DepartmentAddress VARCHAR(255) NOT NULL,
    Amount DECIMAL(18, 2) NOT NULL,
    Currency VARCHAR(10) NOT NULL,
    Status int DEFAULT 0 NOT NULL
);

CREATE OR REPLACE PROCEDURE insert_order(
    OUT request_id INT, 
    client_id VARCHAR, 
    department_address VARCHAR, 
    amount DECIMAL, 
    currency VARCHAR,
    client_ip VARCHAR DEFAULT NULL,
    status INT DEFAULT 0
)
LANGUAGE plpgsql
AS
$$
BEGIN
    INSERT INTO Orders(ClientId, DepartmentAddress, Amount, Currency, Status, ClientIp)
    VALUES (client_id, department_address, amount, currency, status, client_ip)
    RETURNING Id INTO request_id;
END;
$$;




CREATE OR REPLACE PROCEDURE get_order_by_id(
    OUT client_id VARCHAR, 
    OUT department_address VARCHAR, 
    OUT amount DECIMAL, 
    OUT currency VARCHAR, 
    OUT status INT, 
    OUT client_ip VARCHAR,
    order_id INT
)
LANGUAGE plpgsql
AS
$$
BEGIN
    SELECT Orders.ClientId, Orders.DepartmentAddress, Orders.Amount, Orders.Currency, Orders.Status, Orders.ClientIp
    INTO client_id, department_address, amount, currency, status, client_ip
    FROM Orders
    WHERE Id = order_id;
    
    IF NOT FOUND THEN
        RAISE EXCEPTION 'Order not found with ID %', order_id;
    END IF;
END;
$$;

CREATE OR REPLACE PROCEDURE get_orders(
    OUT result_cursor refcursor,
    client_id_param VARCHAR, 
    department_address_param VARCHAR
)
LANGUAGE plpgsql
AS
$$
BEGIN
    OPEN result_cursor FOR
    SELECT Id, Amount, Currency, Status, ClientIp, DepartmentAddress, ClientId
    FROM Orders
    WHERE ClientId = client_id_param AND DepartmentAddress = department_address_param;
END;
$$;