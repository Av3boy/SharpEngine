import { useAuth0 } from "@auth0/auth0-react";
import { ProfileHeader } from "./UserProfile/ProfileHeader";
import { TabNavigation } from "./UserProfile/TabNavigation";
import { AchievementsTab } from "./UserProfile/AchievementsTab";
import { AssetsTab } from "./UserProfile/AssetsTab";
import { SettingsTab } from "./UserProfile/SettingsTab";
import { useState } from "react";

export default function UserProfilePage() {
  const { isAuthenticated, user } = useAuth0();
  const [activeTab, setActiveTab] = useState('achievements');

  return (
    <div className="min-h-screen bg-gray-50">
      <ProfileHeader />
      <TabNavigation activeTab={activeTab} onTabChange={setActiveTab} />
      
      <div className="py-8">
        {activeTab === 'achievements' && <AchievementsTab />}
        {activeTab === 'assets' && <AssetsTab />}
        {activeTab === 'settings' && <SettingsTab />}
      </div>
    </div>
  );
}