import Cookies from "universal-cookie";
import "./NavBar.css";

export default function NavBar(props) {
    return (
        <div className="nav-bar">
            <div>Hello {props.userNickname}!</div>
            <div onClick={e => {
                const cookies = new Cookies();
                cookies.remove('email');
                props.setUserNickname('');
            }}>
                Log out
            </div>
        </div>
    )
}