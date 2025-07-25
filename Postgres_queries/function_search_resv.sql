--DROP FUNCTION booking.search_reservations(text,text)

CREATE OR REPLACE FUNCTION booking.search_reservations(
    query text,
    fallback text
)
RETURNS TABLE (
    id int,
    guestname text,
    checkin date,
    checkout date,
    totalprice numeric,
    packageid int,
    packageName varchar(255),
    packageDescription text,
    packageBasePrice numeric
)
AS $$
BEGIN
    RETURN QUERY
    SELECT 
        r.id,
        r.guestname,
        r.checkin,
        r.checkout,
        r.totalprice,
        r.packageid,
        p.name,
        p.description,
        p.price
    FROM booking.reservations r
    JOIN catalog.packages p ON r.packageid = p.id
    WHERE r.guest_search_vector @@ to_tsquery('english', query)
       OR r.guestname ILIKE fallback;
END;
$$ LANGUAGE plpgsql STABLE;
