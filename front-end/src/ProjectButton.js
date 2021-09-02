import React from 'react';
import { Link } from 'react-router-dom';

import './ProjectButton.css'

class ProjectButton extends React.Component {
  render() {
    return (
        <Link className="project-container" to={'projects/' + this.props.guid} >
            <div>{this.props.projectName}</div>
            <div>{this.props.projectOwner}</div>
        </Link>
    );
  }
}

export default ProjectButton;