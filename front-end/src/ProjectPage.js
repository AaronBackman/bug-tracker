import React from 'react';

class ProjectPage extends React.Component {
  render() {
    console.log(this.props);

    return (
      <div>
        {this.props.match.params.guid}
      </div>
    );
  }
}

export default ProjectPage;