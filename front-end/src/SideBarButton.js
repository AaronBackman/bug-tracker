import "./SideBarButton.css";

export default function SideBarButton(props) {
    return (
        <div className="sidebar-button-container">
            <div className="sidebar-button">{props.text}</div>
        </div>
    )
}