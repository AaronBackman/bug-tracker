import React from 'react';
import Cookies from 'universal-cookie';

const axios = require('axios');


class Login extends React.Component {
  state = {
    email: ''
  }

  render() {
    const cookies = new Cookies();

    if (cookies.get('email')) {
      this.props.history.push('/home');
    }

    return (
      <div>
        <form>
            <input type="text" value={this.state.email} onChange={e => this.setState({email: e.target.value})} />
        </form>
        <div onClick={e => {
          e.preventDefault();
          
          axios.get('https://localhost:5000/api/user/' + this.state.email)
          .then(response => {
            console.log(response.data);
  
            if (response.data) {
              cookies.set('email', this.state.email, {path: '/', sameSite: 'strict'});
              this.props.history.push('/home');
            }
            else {
              console.log('User not found');
            }
          })
          .catch(error => {
              console.log(error);
          })
        }}>ready</div>
      </div>
    );
  }
}

export default Login;