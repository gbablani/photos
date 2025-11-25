# Photo Memories Enhancement App

A cross-platform photo enhancement application that helps nostalgia seekers and family archivists rediscover and rejuvenate old memories.

## Features

### Smart Search & Browse
- **Text Prompt Search**: Search photos by description, tags, or date
- **People & Tags**: Filter photos by person using face recognition
- **Date Navigation**: Browse photos by timeline

### Photo Enhancement Toolkit
- **Colorize B&W**: Transform black & white photos into natural-looking color images
- **Restore Quality**: Enhance faded or low-resolution images
- **Lighting Corrections**: Auto-apply brightness and contrast improvements

### Photo-to-Clip Animation (Signature Feature)
- **Single Photo Animation**: Ken Burns effect, parallax motion, or subtle movements
- **Multi-Photo Montage**: Create dynamic slideshows with transitions and music

### Advanced Video Enhancements
- **Add Person to Video**: Integrate a person from a photo into a video
- **Extend Video Length**: Intelligently extend short video clips
- **Video Quality Upscale**: Stabilize, color-correct, and upscale old videos

### External Integrations
- **Google Photos**: Import and export photos
- **Microsoft OneDrive**: Sync with OneDrive storage
- Coming soon: Dropbox, iCloud Photos

## Tech Stack

### Backend
- **Runtime**: .NET 9.0 (C#)
- **Framework**: ASP.NET Core Web API
- **Database**: Entity Framework Core (SQL Server / In-Memory)
- **Storage**: Azure Blob Storage
- **Authentication**: JWT with OAuth (Google, Microsoft, Facebook)

### Frontend
- **Framework**: React 18 with TypeScript
- **Build Tool**: Vite
- **State Management**: Zustand
- **API Client**: Axios with React Query
- **Routing**: React Router v7

## Getting Started

### Prerequisites
- .NET 9.0 SDK
- Node.js 18+
- (Optional) Azure Storage Emulator or Azurite

### Backend Setup

```bash
cd src/backend/PhotoMemories.Api
dotnet restore
dotnet run
```

The API will be available at `https://localhost:5001` (or `http://localhost:5000`).

### Frontend Setup

```bash
cd src/frontend
npm install
npm run dev
```

The web app will be available at `http://localhost:5173`.

## API Endpoints

### Authentication
- `POST /api/auth/external` - Authenticate with external provider
- `GET /api/auth/me` - Get current user
- `PATCH /api/auth/me` - Update user profile

### Photos
- `GET /api/photos` - List photos with search/filter
- `GET /api/photos/{id}` - Get photo details
- `POST /api/photos` - Upload a new photo
- `PATCH /api/photos/{id}` - Update photo metadata
- `DELETE /api/photos/{id}` - Delete a photo

### Albums
- `GET /api/albums` - List albums
- `GET /api/albums/{id}` - Get album with photos
- `POST /api/albums` - Create album
- `POST /api/albums/{id}/photos` - Add photos to album

### Enhancements
- `GET /api/enhancements/subscription` - Get subscription status
- `POST /api/enhancements/colorize/{photoId}` - Colorize a photo
- `POST /api/enhancements/restore/{photoId}` - Restore photo quality
- `POST /api/enhancements/animate/{photoId}` - Create photo animation
- `POST /api/enhancements/montage` - Create photo montage
- `GET /api/enhancements/jobs` - List enhancement jobs

### Integrations
- `POST /api/integrations/google-photos/connect` - Connect Google Photos
- `GET /api/integrations/google-photos/browse` - Browse Google Photos
- `POST /api/integrations/google-photos/import` - Import from Google Photos
- `POST /api/integrations/onedrive/connect` - Connect OneDrive
- `GET /api/integrations/onedrive/browse` - Browse OneDrive

## Monetization

### Free Tier
- 2 free enhancement operations
- Unlimited browsing and organizing

### Pay As You Go
- Purchase credit packs (10, 25, 50, 100 credits)
- Credits never expire

### Premium Subscription
- Unlimited enhancements
- Priority processing
- Early access to new features

## Project Structure

```
src/
├── backend/
│   └── PhotoMemories.Api/
│       ├── Controllers/     # API endpoints
│       ├── Models/          # Domain entities
│       ├── DTOs/            # Data transfer objects
│       ├── Services/        # Business logic
│       └── Data/            # Database context
└── frontend/
    └── src/
        ├── api/             # API client
        ├── components/      # React components
        ├── pages/           # Page components
        ├── store/           # State management
        └── types/           # TypeScript types
```

## Configuration

### Backend (appsettings.json)
```json
{
  "Jwt": {
    "Key": "your-secret-key",
    "Issuer": "PhotoMemories",
    "Audience": "PhotoMemoriesApp"
  },
  "ConnectionStrings": {
    "DefaultConnection": "your-sql-connection",
    "BlobStorage": "your-azure-storage-connection"
  }
}
```

### Frontend (.env)
```
VITE_API_URL=http://localhost:5000/api
```

## License

Copyright © 2024 Photo Memories
