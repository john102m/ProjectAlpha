# 📦 ProjectAlpha: Local Dev Environment

Your personal cheat sheet for Docker, Postgres, backups, and development sanity.

---

## 🔧 Services & Ports

| Service             | Port (Host) | Description                          |
|---------------------|-------------|--------------------------------------|
| API Gateway (Docker) | `5000`      | Routes to all microservices          |
| VS2022 Dev Gateway   | `5028`      | Debugging gateway during dev         |
| Postgres (Docker)    | `5433`      | Bind-mounted DB container            |
| RabbitMQ             | `5672 / 15672` | Broker + management UI             |

---

## 📁 Data Storage

- Postgres stores data here: ./data/postgres

- Docker Compose bind mount:
```yaml
volumes:
  - ./data/postgres:/var/lib/postgresql/data
⚠️ This folder holds your actual database. Deleting it = lost data.

💾 Backup & Restore
pgAdmin Steps
Backup:

Right-click database → Backup

Choose format: Custom (.backup) or Plain (.sql)

Save to ./backups/ or other known folder

Restore:

Right-click database → Restore

Select the file and matching format

Command Line
Backup:

bash
pg_dump -h localhost -p 5433 -U cataloguser catalogdb > backup.sql
Restore:

bash
psql -h localhost -p 5433 -U cataloguser catalogdb < backup.sql



