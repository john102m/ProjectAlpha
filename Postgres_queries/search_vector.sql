--ALTER TABLE booking.reservations ADD COLUMN guest_search_vector tsvector;
--UPDATE booking.reservations SET guest_search_vector = to_tsvector('english', guestname);

--CREATE INDEX idx_guest_search ON booking.reservations USING GIN (guest_search_vector);

-- SELECT
-- 	*
-- FROM
-- 	BOOKING.RESERVATIONS
-- WHERE
-- 	GUEST_SEARCH_VECTOR @@ TO_TSQUERY('english', 'olivia');

-- SELECT 
--   r.id, r.guestname, r.checkin, r.checkout, r.totalprice,
--   r.packageid,
--   p.name AS packageName, p.description AS packageDescription, p.price AS packageBasePrice
-- FROM booking.reservations r
-- JOIN catalog.packages p ON r.packageid = p.id
-- WHERE r.guest_search_vector @@ to_tsquery('english', 'olivia');



