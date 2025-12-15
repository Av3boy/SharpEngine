import { ImageWithFallback } from './figma/ImageWithFallback';
import { FileImage, Download, Heart, Calendar } from 'lucide-react';

interface Asset {
  id: string;
  title: string;
  type: string;
  imageUrl: string;
  publishedDate: string;
  downloads: number;
  likes: number;
}

const mockAssets: Asset[] = [
  {
    id: '1',
    title: 'Modern Dashboard UI Kit',
    type: 'UI Kit',
    imageUrl: 'https://images.unsplash.com/photo-1625009431843-18569fd7331b?crop=entropy&cs=tinysrgb&fit=max&fm=jpg&ixid=M3w3Nzg4Nzd8MHwxfHNlYXJjaHwxfHxkZXNpZ24lMjBtb2NrdXB8ZW58MXx8fHwxNzY1NzM1MzcyfDA&ixlib=rb-4.1.0&q=80&w=1080&utm_source=figma&utm_medium=referral',
    publishedDate: '2024-11-20',
    downloads: 1243,
    likes: 456,
  },
  {
    id: '2',
    title: 'Web Application Components',
    type: 'Component Library',
    imageUrl: 'https://images.unsplash.com/photo-1704699175212-117f10d5b3b4?crop=entropy&cs=tinysrgb&fit=max&fm=jpg&ixid=M3w3Nzg4Nzd8MHwxfHNlYXJjaHwxfHx3ZWIlMjBpbnRlcmZhY2V8ZW58MXx8fHwxNzY1NzM1MzczfDA&ixlib=rb-4.1.0&q=80&w=1080&utm_source=figma&utm_medium=referral',
    publishedDate: '2024-12-05',
    downloads: 892,
    likes: 312,
  },
  {
    id: '3',
    title: 'Mobile App Design System',
    type: 'Design System',
    imageUrl: 'https://images.unsplash.com/photo-1609921212029-bb5a28e60960?crop=entropy&cs=tinysrgb&fit=max&fm=jpg&ixid=M3w3Nzg4Nzd8MHwxfHNlYXJjaHwxfHxtb2JpbGUlMjBhcHAlMjBkZXNpZ258ZW58MXx8fHwxNzY1NzE5MzgzfDA&ixlib=rb-4.1.0&q=80&w=1080&utm_source=figma&utm_medium=referral',
    publishedDate: '2024-12-12',
    downloads: 567,
    likes: 234,
  },
];

export function AssetsTab() {
  return (
    <div className="max-w-6xl mx-auto p-8">
      <div className="flex items-center gap-3 mb-6">
        <FileImage className="w-6 h-6 text-blue-600" />
        <h2>Published Assets</h2>
        <span className="text-gray-500">({mockAssets.length} assets)</span>
      </div>
      
      <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
        {mockAssets.map((asset) => (
          <div
            key={asset.id}
            className="border border-gray-200 rounded-lg overflow-hidden hover:shadow-lg transition-shadow"
          >
            <div className="aspect-video bg-gray-100 overflow-hidden">
              <ImageWithFallback
                src={asset.imageUrl}
                alt={asset.title}
                className="w-full h-full object-cover"
              />
            </div>
            
            <div className="p-4">
              <div className="mb-1">
                <span className="text-xs text-blue-600 bg-blue-50 px-2 py-1 rounded">
                  {asset.type}
                </span>
              </div>
              
              <h3 className="mb-2">{asset.title}</h3>
              
              <div className="flex items-center gap-2 text-sm text-gray-500 mb-3">
                <Calendar className="w-4 h-4" />
                <span>
                  {new Date(asset.publishedDate).toLocaleDateString('en-US', {
                    month: 'short',
                    day: 'numeric',
                    year: 'numeric',
                  })}
                </span>
              </div>
              
              <div className="flex items-center justify-between text-sm">
                <div className="flex items-center gap-1 text-gray-600">
                  <Download className="w-4 h-4" />
                  <span>{asset.downloads.toLocaleString()}</span>
                </div>
                
                <div className="flex items-center gap-1 text-red-500">
                  <Heart className="w-4 h-4" />
                  <span>{asset.likes.toLocaleString()}</span>
                </div>
              </div>
            </div>
          </div>
        ))}
      </div>
    </div>
  );
}
