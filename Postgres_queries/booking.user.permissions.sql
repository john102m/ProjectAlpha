--GRANT SELECT, INSERT, UPDATE, DELETE ON TABLE booking.reservations TO bookinguser;

GRANT USAGE, SELECT ON SEQUENCE booking.reservations_id_seq TO bookinguser;
