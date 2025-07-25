--SELECT to_tsvector('english', 'Olivia Trenton is traveling with friends');
-- Output: 'friend':6 'olivia':1 'travel':4 'trenton':2
SELECT to_tsquery('english', 'olivia & trenton');



