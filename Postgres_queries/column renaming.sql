-- Migration: Rename 'product_id' to 'package' in booking.reservations
DO $$
BEGIN
    -- Only rename if the old column exists and new one doesn't
    IF EXISTS (
        SELECT 1 FROM information_schema.columns 
        WHERE table_schema = 'booking' AND table_name = 'reservations' AND column_name = 'product_id'
    ) AND NOT EXISTS (
        SELECT 1 FROM information_schema.columns 
        WHERE table_schema = 'booking' AND table_name = 'reservations' AND column_name = 'package'
    ) THEN
        ALTER TABLE booking.reservations
        RENAME COLUMN product_id TO package;
    END IF;
END $$;
