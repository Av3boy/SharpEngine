
// import LogoutIcon from '@mui/icons-material/Logout';
import { User } from "@auth0/auth0-react";

export type HeaderLoginProps = {
  onProfileClicked: () => void;
  loginWithRedirect: () => void | Promise<void>;
  logout: () => void | Promise<void>;
  isAuthenticated: boolean;
  user?: User;
  useWithText?: boolean;
};

type AuthenticatedActionsProps = {
  onProfileClicked: () => void;
  logout: () => void | Promise<void>;
  useWithText?: boolean;
};

function AuthenticatedActions({ onProfileClicked, logout, useWithText }: AuthenticatedActionsProps) {
  return (
    <>
      <button
        className="flex items-center gap-2 px-4 py-2 bg-white/10 hover:bg-white/20 rounded-lg transition-colors"
        onClick={onProfileClicked}
      >
        {/*<UserIcon className={`w-5 h-5 ${useWithText ? "text-white" : ""}`} />*/}
        <span className={` ${useWithText ? "text-white" : ""}`}>Profile</span>
      </button>
      <button onClick={() => logout()}>Logout</button>
    </>
  );
}

type UnauthenticatedActionsProps = {
  loginWithRedirect: () => void | Promise<void>;
  useWithText?: boolean;
};

function UnauthenticatedActions({ loginWithRedirect, useWithText }: UnauthenticatedActionsProps) {
  return (
    <button
      className={`px-6 py-2 hover:text-blue-400 transition-colors ${useWithText ? "text-white" : ""}`}
      onClick={() => loginWithRedirect()}
    >
      Login
    </button>
  );
}

export function HeaderLogin({onProfileClicked, loginWithRedirect, logout, isAuthenticated, user, useWithText}: HeaderLoginProps) {
  return (
    <div className="flex items-center gap-4">
      {isAuthenticated ? (
        <AuthenticatedActions onProfileClicked={onProfileClicked} logout={logout} useWithText={useWithText} />
      ) : (
        <UnauthenticatedActions loginWithRedirect={loginWithRedirect} useWithText={useWithText} />
      )}
    </div>
  );
}