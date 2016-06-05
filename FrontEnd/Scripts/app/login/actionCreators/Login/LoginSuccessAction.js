import {browserHistory} from 'react-router';
import * as toastr from 'toastr';

import {LOGIN_SUCCESS} from '../../constants/LoginConstant';

export function LoginSuccessAction(token){
    browserHistory.push('/');
    toastr.success('LogIn successfully!');
    localStorage.setItem("access_token",token);
    
    return {
        type: LOGIN_SUCCESS,
        data: token
    };
}