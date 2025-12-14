
import LogoutIcon from '@mui/icons-material/Logout';
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
        className="flex items-center gap-8 px-4 text-white/80 hover:text-white transition-colors"
        onClick={onProfileClicked}
      >
        {/*<UserIcon className={`w-5 h-5 ${useWithText ? "text-white" : ""}`} />*/}
        <span className="text-white/80 hover:text-white transition-colors">Profile</span>
      </button>
      <button className="text-white/80 hover:text-white transition-colors" onClick={() => logout()}>
      <svg xmlns="http://www.w3.org/2000/svg" enable-background="new 0 0 24 24" height="24px" viewBox="0 0 24 24" width="24px" fill="#e3e3e3"><g><path d="M0,0h24v24H0V0z" fill="none"/></g><g><path d="M17,8l-1.41,1.41L17.17,11H9v2h8.17l-1.58,1.58L17,16l4-4L17,8z M5,5h7V3H5C3.9,3,3,3.9,3,5v14c0,1.1,0.9,2,2,2h7v-2H5V5z"/></g></svg>
      </button>
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
      className="text-white/80 hover:text-white transition-colors"
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