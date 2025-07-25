-- Create the user
CREATE USER bookinguser WITH PASSWORD 'bookingpass';

-- Create the schema owned by bookinguser
CREATE SCHEMA booking AUTHORIZATION bookinguser;

-- Grant access (just to be sure)
GRANT USAGE ON SCHEMA booking TO bookinguser;
GRANT SELECT, INSERT, UPDATE, DELETE ON ALL TABLES IN SCHEMA booking TO bookinguser;
--REVOKE ALL ON SCHEMA public FROM bookinguser;
--REVOKE ALL PRIVILEGES ON ALL TABLES IN SCHEMA public FROM bookinguser;
