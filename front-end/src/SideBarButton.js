import { Link } from "react-router-dom";

import "./SideBarButton.css";

export default function SideBarButton(props) {
    return (
        <Link className="sidebar-button-container" to={props.path}>
            <div className="sidebar-button">{props.text}</div>
        </Link>
    )
}