DO $$
BEGIN
    IF EXISTS (
        SELECT 1 FROM information_schema.tables 
        WHERE table_schema = 'catalog' AND table_name = 'products'
    ) AND NOT EXISTS (
        SELECT 1 FROM information_schema.tables 
        WHERE table_schema = 'catalog' AND table_name = 'packages'
    ) THEN
        ALTER TABLE catalog.products RENAME TO packages;
    END IF;
END $$;
