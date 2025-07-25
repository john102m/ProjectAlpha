SELECT
  r.id,
  r.guestname,
  r.checkin,
  r.checkout,
  r.totalprice,
  p.name AS package_name,
  p.description AS package_description,
  p.price AS package_price
FROM booking.reservations r
JOIN catalog.packages p ON r.packageid = p.id;
