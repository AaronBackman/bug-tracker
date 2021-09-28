import React from "react";

import SideBarButton from "./SideBarButton";

import "./SideBar.css";

class SideBar extends React.Component {
  render() {
    return (
      <div className="side-bar">
        <SideBarButton path="/dashboard" text="Dashboard" />
        <SideBarButton path="/projects" text="Projects" />
      </div>
    );
  }
}

export default SideBar;
