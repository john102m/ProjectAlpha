SELECT column_name, data_type, character_maximum_length
FROM information_schema.columns
WHERE table_schema = 'catalog' AND table_name = 'packages';
