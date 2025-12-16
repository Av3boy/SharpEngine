interface Achievement {
  id: string;
  title: string;
  description: string;
  imageUrl: string;
  earnedDate: string;
  rarityPercentage: number;
}

const mockAchievements: Achievement[] = [
  {
    id: '1',
    title: 'First Victory',
    description: 'Complete your first project',
    imageUrl: 'https://images.unsplash.com/photo-1762345127396-ac4a970436c3?crop=entropy&cs=tinysrgb&fit=max&fm=jpg&ixid=M3w3Nzg4Nzd8MHwxfHNlYXJjaHwxfHx0cm9waHklMjBhY2hpZXZlbWVudHxlbnwxfHx8fDE3NjU3MzUzNDh8MA&ixlib=rb-4.1.0&q=80&w=1080&utm_source=figma&utm_medium=referral',
    earnedDate: '2024-11-15',
    rarityPercentage: 85.2,
  },
  {
    id: '2',
    title: 'Gold Contributor',
    description: 'Contribute 100 assets to the community',
    imageUrl: 'https://images.unsplash.com/photo-1720592592437-0c679716ac48?crop=entropy&cs=tinysrgb&fit=max&fm=jpg&ixid=M3w3Nzg4Nzd8MHwxfHNlYXJjaHwxfHxnb2xkJTIwbWVkYWwlMjB3aW5uZXJ8ZW58MXx8fHwxNzY1Njg1MDAzfDA&ixlib=rb-4.1.0&q=80&w=1080&utm_source=figma&utm_medium=referral',
    earnedDate: '2024-12-01',
    rarityPercentage: 12.5,
  },
  {
    id: '3',
    title: 'Rising Star',
    description: 'Receive 1000 likes on your work',
    imageUrl: 'https://images.unsplash.com/photo-1763663523128-0bc29ffdbb73?crop=entropy&cs=tinysrgb&fit=max&fm=jpg&ixid=M3w3Nzg4Nzd8MHwxfHNlYXJjaHwxfHxzdGFyJTIwYmFkZ2V8ZW58MXx8fHwxNzY1NzM1MzQ5fDA&ixlib=rb-4.1.0&q=80&w=1080&utm_source=figma&utm_medium=referral',
    earnedDate: '2024-12-10',
    rarityPercentage: 28.7,
  },
  {
    id: '4',
    title: 'Champion',
    description: 'Win a design competition',
    imageUrl: 'https://images.unsplash.com/photo-1739323980445-b76a6e459d0b?crop=entropy&cs=tinysrgb&fit=max&fm=jpg&ixid=M3w3Nzg4Nzd8MHwxfHNlYXJjaHwxfHxjcm93biUyMGNoYW1waW9ufGVufDF8fHx8MTc2NTczNTM0OXww&ixlib=rb-4.1.0&q=80&w=1080&utm_source=figma&utm_medium=referral',
    earnedDate: '2024-12-14',
    rarityPercentage: 5.3,
  },
];

function getRarityColor(percentage: number): string {
  if (percentage < 10) return 'text-purple-600';
  if (percentage < 25) return 'text-blue-600';
  if (percentage < 50) return 'text-green-600';
  return 'text-gray-600';
}

function getRarityLabel(percentage: number): string {
  if (percentage < 10) return 'Legendary';
  if (percentage < 25) return 'Rare';
  if (percentage < 50) return 'Uncommon';
  return 'Common';
}

export function AchievementsTab() {
  return (
    <div className="max-w-6xl mx-auto p-8">
      <div className="flex items-center gap-3 mb-6">
        {/*<Trophy className="w-6 h-6 text-blue-600" />*/}
        <h2>Your Achievements</h2>
        <span className="text-gray-500">({mockAchievements.length} earned)</span>
      </div>
      
      <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
        {mockAchievements.map((achievement) => (
          <div
            key={achievement.id}
            className="border border-gray-200 rounded-lg p-6 hover:shadow-lg transition-shadow"
          >
            <div className="flex gap-4">
              <div className="w-24 h-24 flex-shrink-0 rounded-lg overflow-hidden bg-gray-100">
                {/*<ImageWithFallback
                  src={achievement.imageUrl}
                  alt={achievement.title}
                  className="w-full h-full object-cover"
                />*/}
              </div>
              
              <div className="flex-1">
                <h3 className="mb-1">{achievement.title}</h3>
                <p className="text-gray-600 mb-3">{achievement.description}</p>
                
                <div className="flex items-center justify-between text-sm">
                  <div className="text-gray-500">
                    Earned: {new Date(achievement.earnedDate).toLocaleDateString('en-US', {
                      month: 'short',
                      day: 'numeric',
                      year: 'numeric',
                    })}
                  </div>
                  
                  <div className={`flex items-center gap-1 ${getRarityColor(achievement.rarityPercentage)}`}>
                    {/*<Users className="w-4 h-4" />*/}
                    <span>
                      {achievement.rarityPercentage}% ({getRarityLabel(achievement.rarityPercentage)})
                    </span>
                  </div>
                </div>
              </div>
            </div>
          </div>
        ))}
      </div>
    </div>
  );
}
