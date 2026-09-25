# News Backend API — How to run

This guide walks through everything needed to run the API from a fresh clone,
configure it, apply migrations, load demo data, and test it in Swagger.

---

## 1. Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet) (matches the
  `TargetFramework` in the project files)
- A SQL Server you can connect to — e.g. a local **SQL Server Developer /
  Express** instance, **LocalDB**, or a Docker container:

  ```bash
  docker run --name newsdb -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=Your_strong_Pass1" -p 1433:1433 -d mcr.microsoft.com/mssql/server:2022-latest
  ```

- (Optional) A [Cloudinary](https://cloudinary.com) account if you want profile
  photo uploads to work.

---

## 2. Clone

```bash
git clone <repository-url>
cd news-app-backend
```

---

## 3. Configure secrets

The API reads its database connection and JWT signing key from configuration.
The simplest local option is **.NET user secrets** (development only).

```bash
cd NewsBackend.API

dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=localhost;Database=NewsDb;User Id=sa;Password=Your_strong_Pass1;TrustServerCertificate=True"
dotnet user-secrets set "Jwt:Secret" "some-long-random-secret-with-32-characters-minimum"
```

> For LocalDB instead of a TCP server, use:
> `Server=(localdb)\MSSQLLocalDB;Database=NewsDb;Trusted_Connection=True;MultipleActiveResultSets=true`
>
> For non-development environments, export the same values as environment
> variables using `__` as separator (`ConnectionStrings__DefaultConnection`,
> `Jwt__Secret`, ...). See [`.env.example`](.env.example).

Alternatively `dotnet run` on a Linux/macOS shell after exporting:

```bash
export ConnectionStrings__DefaultConnection="Server=localhost;Database=NewsDb;User Id=sa;Password=Your_strong_Pass1;TrustServerCertificate=True"
export Jwt__Secret="some-long-random-secret-with-32-characters-minimum"
```

---

## 4. Restore tools and dependencies

```bash
cd ..   # back to the repo root

dotnet restore
dotnet tool restore            # installs the EF Core CLI (if not already installed)
```

---

## 5. Apply migrations

```bash
dotnet ef database update --project NewsBackend.API
```

This creates the database (if needed) and applies every EF Core migration.

---

## 6. Run the API

```bash
dotnet run --project NewsBackend.API
```

The launch profile listens on:

- HTTP  : http://localhost:5191
- HTTPS : https://localhost:7200 (also serves HTTP on 5191)

Useful URLs:

| URL | Purpose |
| --- | --- |
| `http://localhost:5191/swagger` | Swagger UI (Development only) |
| `http://localhost:5191/openapi/v1.json` | Raw OpenAPI document |
| `http://localhost:5191/api/v1/health` | Liveness check |

---

## 7. Demo data

If the connected database is **empty**, the app seeds it automatically on
startup (`Development` environment only):

- 3 users — Admin, Editor, User
- 5 categories — Technology, Sports, Business, Science, Entertainment
- 8 published + 3 draft articles with placeholder images
- 4 comments and 3 bookmarks

Demo credentials (all `Password123`):

| Role | Email |
| --- | --- |
| Admin | `admin@demo.news` |
| Editor | `editor@demo.news` |
| User | `user@demo.news` |

```
Seeding is skipped if the database already contains any users.
```

---

## 8. Verify the API

```bash
# Health
curl http://localhost:5191/api/v1/health

# Login as the demo user
curl -X POST http://localhost:5191/api/v1/auth/login \
  -H "Content-Type: application/json" \
  -d '{"email":"user@demo.news","password":"Password123"}'

# List published articles
curl http://localhost:5191/api/v1/articles
```

---

## 9. Try it in Swagger

1. Open `http://localhost:5191/swagger`.
2. Call `POST /auth/login` with a demo account and copy `accessToken`.
3. Click **Authorize**, paste the token, and apply it.
4. Call e.g. `GET /users/me` — the `Authorization: Bearer <token>` header is
   added automatically.

---

## 10. Configure Cloudinary (optional)

```bash
cd NewsBackend.API
dotnet user-secrets set "Cloudinary:CloudName" "..."
dotnet user-secrets set "Cloudinary:ApiKey" "..."
dotnet user-secrets set "Cloudinary:ApiSecret" "..."
```

Without these, every endpoint works except `POST /users/me/photo`, which will
return `500` with `Cloudinary is not configured`. See the
[README section on Cloudinary](README.md#12-cloudinary-configuration).

---

## 11. Troubleshooting

- **`error MSB3021 ... file is locked`** — another `NewsBackend.API` process is
  still running. Stop it, then rebuild: `taskkill /IM NewsBackend.API.exe /F` on
  Windows.
- **`Cannot open database ... requested by the login`** — the database does not
  exist yet, or your login lacks access. Run `dotnet ef database update` once and
  check the connection string.
- **`401` on everything** — the `Jwt:Secret` differs between the process that
  issued the token and the one validating it (e.g. different user-secrets scope).
- **Swagger 404** — run in the `Development` environment
  (`ASPNETCORE_ENVIRONMENT=Development`); the launch profile sets this already.