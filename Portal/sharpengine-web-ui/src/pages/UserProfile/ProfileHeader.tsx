import { User } from 'lucide-react';

export function ProfileHeader() {
  return (
    <div className="bg-gradient-to-r from-blue-600 to-purple-600 text-white p-8">
      <div className="max-w-6xl mx-auto flex items-center gap-6">
        <div className="w-24 h-24 rounded-full bg-white/20 flex items-center justify-center">
          <User className="w-12 h-12" />
        </div>
        <div>
          <h1>John Doe</h1>
          <p className="text-white/80">Member since December 2024</p>
        </div>
      </div>
    </div>
  );
}
