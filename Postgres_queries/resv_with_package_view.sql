CREATE VIEW booking.v_reservations_with_package_info AS
SELECT 
    r.id,
    r.guestname,
    r.checkin,
    r.checkout,
    r.totalprice,
    r.packageid,
    p.name AS packageName,
    p.description AS packageDescription,
    p.price AS packageBasePrice
FROM booking.reservations r
JOIN catalog.packages p ON r.packageid = p.id;
