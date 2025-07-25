-- Add a column referencing the catalog product
--ALTER TABLE booking.reservations
--ADD COLUMN product_id INT NOT NULL DEFAULT 1;

-- Add foreign key constraint pointing to catalog.products
--ALTER TABLE booking.reservations
--ADD CONSTRAINT fk_product
--FOREIGN KEY (product_id)
--REFERENCES catalog.products(id);


-- Update all existing rows to have valid product references
UPDATE booking.reservations SET product_id = 1;