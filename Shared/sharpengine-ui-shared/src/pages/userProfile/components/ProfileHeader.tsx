import PermIdentityIcon from '@mui/icons-material/PermIdentity';

export function ProfileHeader() {
  return (
    <div style={{backgroundImage: "linear-gradient(to right, oklch(0.546 0.245 262.881) 0%, oklch(0.558 0.288 302.321) 100%)"}} className="text-black p-8">
      <div className="max-w-6xl mx-auto flex items-center gap-6">
        <div className="w-24 h-24 rounded-full bg-white/20 flex items-center justify-center">
          <PermIdentityIcon style={{ fontSize: 64 }} />
        </div>
        <div>
          <h1 className="text-white">John Doe</h1>
          <p className="text-white/80">Member since December 2024</p>
        </div>
      </div>
    </div>
  );
}
