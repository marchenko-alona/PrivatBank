#!/bin/bash

/opt/mssql/bin/sqlservr &

echo "Waiting for SQL Server to start..."
until /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P $MSSQL_SA_PASSWORD -C -Q "SELECT 1"; do
  sleep 5
done

echo "SQL Server is up. Running initialization script..."

if [ -f /scripts/init.sql ]; then
  /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P $MSSQL_SA_PASSWORD -C -d master -i /scripts/init.sql
  echo "Initialization script executed successfully."
else
  echo "No initialization script found."
fi

wait

