import React from 'react';
import Cookies from 'universal-cookie';

import Project from './Project';

const axios = require('axios');

class ProjectList extends React.Component {
  state = {
    projectList: [],
    newProjectName: ''
  };

  componentDidMount() {
    console.log(this.props.user);
    const cookies = new Cookies();
    axios.get('https://localhost:5000/api/projects?email=' + cookies.get('email'))
      .then(response => {
        console.log(response.data);
        this.setState({projectList: response.data});
      })
      .catch(error => {
          console.log(error);
      })
  }
    
  render() {
    return (
      <div>
        {this.state.projectList.map(project => <Project key={project.projectGUID} projectName={project.projectName} projectOwner={project.ownerNickname} />)}
        <div onClick={() => {
          axios.post('https://localhost:5000/api/projects?email=' + this.props.email, {projectName: this.state.newProjectName})
          .then(response => {
            console.log(response.data);
            const project = response.data;
            project.nickname = this.props.user.nickname;
            this.setState({projectList: this.state.projectList.concat(project)});
          })
          .catch(error => {
              console.log(error);
          })
        }}>
          New Project
        </div>
        <form>
            <input type="text" value={this.state.newProjectName} onChange={e => this.setState({newProjectName: e.target.value})} />
        </form>
      </div>
    );
  }
}

export default ProjectList;