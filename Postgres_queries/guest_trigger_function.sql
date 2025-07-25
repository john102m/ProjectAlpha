CREATE OR REPLACE FUNCTION update_guest_search_vector()
RETURNS TRIGGER AS $$
BEGIN
    NEW.guest_search_vector := to_tsvector('english', NEW.guestname);
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;
