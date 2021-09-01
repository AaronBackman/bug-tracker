import React from 'react';

class Project extends React.Component {
  render() {
    return (
        <div>
            <div>{this.props.projectName}</div>
            <div>{this.props.projectOwner}</div>
        </div>
    );
  }
}

export default Project;