-- CREATE TRIGGER trigger_update_guest_search_vector
-- BEFORE INSERT OR UPDATE ON booking.reservations
-- FOR EACH ROW
-- EXECUTE FUNCTION update_guest_search_vector();

-- UPDATE booking.reservations 
-- SET guest_search_vector = to_tsvector('english', guestname);

select * from booking.reservations

