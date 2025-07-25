CREATE TABLE booking.reservations (
    id SERIAL PRIMARY KEY,
    guestname TEXT NOT NULL,
    checkin DATE NOT NULL,
    checkout DATE NOT NULL,
    destination TEXT NOT NULL,
    totalprice NUMERIC(10, 2) NOT NULL
);
