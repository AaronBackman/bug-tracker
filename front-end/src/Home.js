import React from 'react';
import Cookies from 'universal-cookie';
import axios from 'axios';
import {BrowserRouter as Router, Switch, Route, Redirect} from 'react-router-dom';

import SideBarButton from './SideBarButton.js';
import ProjectList from './ProjectList.js';

import './Home.css';

class Home extends React.Component {
  state = {
    user: {}
  }

  render() {
    const cookies = new Cookies();
    if (!cookies.get('email')) {
      this.props.history.push('/login');
    }
    else if (Object.keys(this.state.user).length === 0) {
      axios.get('https://localhost:5000/api/user/' + cookies.get('email'))
        .then(response => {
          console.log(response.data);
          this.setState({user: response.data});
        })
        .catch(error => {
            console.log(error);
        })
    }

    return (
      <div className="window">
        <div className="nav-bar">
          Hello {this.state.user.nickname}!
        </div>
        <div onClick={e => {
          cookies.remove('email');
          this.setState({user: {}});
        }}>
          Log out
        </div>
        <div className="page">
          <div className="side-bar">
            <SideBarButton text="Projects" />
          </div>
          <div className="content">
            <Router>
              <Switch>
                <Route path="/home/projects" exact render={(props) => <ProjectList key="ProjectList" user={this.state.user} {...props} />} />
              </Switch>
            </Router>
          </div>
        </div>
      </div>
    );
  }
}

export default Home;